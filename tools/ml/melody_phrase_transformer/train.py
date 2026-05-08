from __future__ import annotations

import argparse
import json
from dataclasses import asdict, dataclass
from pathlib import Path

import torch
from torch import nn
from torch.utils.data import DataLoader, Dataset, random_split

from model import MelodyPhraseTransformer, MelodyVocabulary


@dataclass(frozen=True)
class TrainConfig:
    embedding_size: int = 128
    heads: int = 4
    layers: int = 2
    feedforward_size: int = 384
    dropout: float = 0.1
    epochs: int = 30
    batch_size: int = 64
    learning_rate: float = 0.0003
    label_smoothing: float = 0.02
    max_sequence_length: int = 96
    max_context_length: int = 16
    validation_ratio: float = 0.15
    seed: int = 1984


class MelodyDataset(Dataset):
    def __init__(self, dataset_path: Path, vocab: MelodyVocabulary, config: TrainConfig) -> None:
        self.examples: list[tuple[list[int], list[int], int]] = []
        self.vocab = vocab
        self.config = config

        for line in dataset_path.read_text(encoding="utf-8").splitlines():
            if not line.strip():
                continue
            item = json.loads(line)
            context = [
                item["style"],
                item["mode"],
                item["mood"],
                item["meter"],
                f"BARS_{item['bars']}",
                *item.get("progression", []),
            ][: config.max_context_length]
            context_ids = [vocab.encode(token) for token in context]
            tokens = [vocab.encode(token) for token in item["tokens"]]

            for index in range(1, len(tokens)):
                previous = tokens[:index][-config.max_sequence_length :]
                target = tokens[index]
                self.examples.append((context_ids, previous, target))

    def __len__(self) -> int:
        return len(self.examples)

    def __getitem__(self, index: int) -> tuple[list[int], list[int], int]:
        return self.examples[index]


def collate_batch(batch: list[tuple[list[int], list[int], int]], pad_id: int) -> tuple[torch.Tensor, ...]:
    max_context = max(len(item[0]) for item in batch)
    max_previous = max(len(item[1]) for item in batch)
    contexts: list[list[int]] = []
    previous_tokens: list[list[int]] = []
    targets: list[int] = []

    for context, previous, target in batch:
        contexts.append(context + [pad_id] * (max_context - len(context)))
        previous_tokens.append(previous + [pad_id] * (max_previous - len(previous)))
        targets.append(target)

    return (
        torch.tensor(contexts, dtype=torch.long),
        torch.tensor(previous_tokens, dtype=torch.long),
        torch.tensor(targets, dtype=torch.long),
    )


def build_output_index_map(vocabulary_size: int, output_token_ids: list[int], device: torch.device) -> torch.Tensor:
    index_map = torch.full((vocabulary_size,), -1, dtype=torch.long, device=device)
    for index, token_id in enumerate(output_token_ids):
        index_map[token_id] = index
    return index_map


def train(args: argparse.Namespace) -> None:
    checkpoint = load_checkpoint(Path(args.resume)) if args.resume else None
    checkpoint_config = checkpoint.get("config") if checkpoint else None
    config = TrainConfig(
        embedding_size=args.embedding_size,
        heads=args.heads,
        layers=args.layers,
        feedforward_size=args.feedforward_size,
        dropout=args.dropout,
        epochs=args.epochs,
        batch_size=args.batch_size,
        learning_rate=args.learning_rate,
        label_smoothing=args.label_smoothing,
        validation_ratio=args.validation_ratio,
        seed=args.seed,
    )
    if checkpoint_config:
        resumed = dict(checkpoint_config)
        resumed["epochs"] = args.epochs
        resumed["batch_size"] = args.batch_size
        resumed["learning_rate"] = args.learning_rate
        resumed["label_smoothing"] = args.label_smoothing
        resumed["validation_ratio"] = args.validation_ratio
        resumed["seed"] = args.seed
        config = TrainConfig(**resumed)

    torch.manual_seed(config.seed)
    vocab = MelodyVocabulary.load(args.vocab)
    dataset = MelodyDataset(Path(args.dataset), vocab, config)
    if len(dataset) == 0:
        raise RuntimeError("Dataset is empty.")

    validation_size = max(1, int(len(dataset) * config.validation_ratio))
    train_size = max(1, len(dataset) - validation_size)
    train_dataset, validation_dataset = random_split(
        dataset,
        [train_size, validation_size],
        generator=torch.Generator().manual_seed(config.seed),
    )
    train_loader = DataLoader(
        train_dataset,
        batch_size=config.batch_size,
        shuffle=True,
        collate_fn=lambda batch: collate_batch(batch, vocab.pad_id),
    )
    validation_loader = DataLoader(
        validation_dataset,
        batch_size=config.batch_size,
        shuffle=False,
        collate_fn=lambda batch: collate_batch(batch, vocab.pad_id),
    )

    device = torch.device("cuda" if torch.cuda.is_available() and not args.cpu else "cpu")
    device_name = torch.cuda.get_device_name(0) if device.type == "cuda" else "cpu"
    print(f"device={device} name={device_name}")

    model = MelodyPhraseTransformer(
        vocabulary_size=len(vocab.id_to_token),
        embedding_size=config.embedding_size,
        heads=config.heads,
        layers=config.layers,
        feedforward_size=config.feedforward_size,
        dropout=config.dropout,
        max_sequence_length=config.max_sequence_length + config.max_context_length,
    ).to(device)
    optimizer = torch.optim.AdamW(model.parameters(), lr=config.learning_rate)
    start_epoch = 0
    best_validation_loss = float("inf")
    metrics: list[dict[str, float | int | str]] = []

    if checkpoint:
        if checkpoint["vocabulary_size"] != len(vocab.id_to_token):
            raise RuntimeError("Checkpoint vocabulary size does not match vocab.json.")
        model.load_state_dict(checkpoint["model_state"])
        if not args.reset_optimizer and "optimizer_state" in checkpoint:
            optimizer.load_state_dict(checkpoint["optimizer_state"])
        start_epoch = int(checkpoint.get("epoch", 0))
        best_validation_loss = float(checkpoint.get("best_validation_loss", float("inf")))
        metrics = load_existing_metrics(Path(args.output_dir))
        print(f"resumed={args.resume} start_epoch={start_epoch} best_val_loss={best_validation_loss:.4f}")

    output_token_ids = torch.tensor(vocab.output_token_ids, dtype=torch.long, device=device)
    output_index_map = build_output_index_map(len(vocab.id_to_token), vocab.output_token_ids, device)
    loss_fn = nn.CrossEntropyLoss(ignore_index=-1, label_smoothing=config.label_smoothing)

    output_dir = Path(args.output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)
    total_target_epoch = start_epoch + config.epochs

    for local_epoch in range(1, config.epochs + 1):
        epoch = start_epoch + local_epoch
        model.train()
        total_loss = 0.0
        total_items = 0
        total_batches = len(train_loader)

        for batch_index, (context_tokens, previous_tokens, targets) in enumerate(train_loader, start=1):
            context_tokens = context_tokens.to(device)
            previous_tokens = previous_tokens.to(device)
            targets = targets.to(device)
            optimizer.zero_grad(set_to_none=True)
            logits = model(context_tokens, previous_tokens).index_select(1, output_token_ids)
            target_classes = output_index_map[targets]
            loss = loss_fn(logits, target_classes)
            loss.backward()
            optimizer.step()

            total_loss += float(loss.item()) * targets.numel()
            total_items += targets.numel()
            if args.progress_every > 0 and (batch_index % args.progress_every == 0 or batch_index == total_batches):
                print(
                    f"train_progress epoch={epoch:03d}/{total_target_epoch:03d} "
                    f"batch={batch_index}/{total_batches} "
                    f"percent={batch_index / max(total_batches, 1) * 100.0:.1f} "
                    f"train_loss={total_loss / max(total_items, 1):.4f}",
                    flush=True,
                )

        train_loss = total_loss / max(total_items, 1)
        validation_loss, accuracy, top3 = evaluate(
            model,
            validation_loader,
            loss_fn,
            device,
            output_token_ids,
            output_index_map,
        )
        improved = validation_loss < best_validation_loss
        if improved:
            best_validation_loss = validation_loss

        metrics.append(
            {
                "epoch": epoch,
                "train_loss": train_loss,
                "validation_loss": validation_loss,
                "accuracy": accuracy,
                "top3_accuracy": top3,
                "best": improved,
            }
        )
        suffix = " best" if improved else ""
        print(
            f"epoch={epoch:03d}/{total_target_epoch:03d} "
            f"train_loss={train_loss:.4f} val_loss={validation_loss:.4f} "
            f"acc={accuracy:.3f} top3={top3:.3f}{suffix}",
            flush=True,
        )
        save_checkpoint(output_dir / "MelodyPhraseTransformer.pt", model, optimizer, epoch, best_validation_loss, config, vocab)
        if improved:
            save_checkpoint(output_dir / "best_model.pt", model, optimizer, epoch, best_validation_loss, config, vocab)
        if args.save_every > 0 and epoch % args.save_every == 0:
            save_checkpoint(output_dir / f"checkpoint_epoch_{epoch:03d}.pt", model, optimizer, epoch, best_validation_loss, config, vocab)
        write_json(output_dir / "metrics.json", metrics)

    write_json(output_dir / "training_config.json", asdict(config))
    print(f"saved={output_dir / 'MelodyPhraseTransformer.pt'}")
    print(f"best={output_dir / 'best_model.pt'}")


@torch.no_grad()
def evaluate(
    model: MelodyPhraseTransformer,
    loader: DataLoader,
    loss_fn: nn.Module,
    device: torch.device,
    output_token_ids: torch.Tensor,
    output_index_map: torch.Tensor,
) -> tuple[float, float, float]:
    model.eval()
    total_loss = 0.0
    total_items = 0
    correct = 0
    top3_correct = 0
    for context_tokens, previous_tokens, targets in loader:
        context_tokens = context_tokens.to(device)
        previous_tokens = previous_tokens.to(device)
        targets = targets.to(device)
        logits = model(context_tokens, previous_tokens).index_select(1, output_token_ids)
        target_classes = output_index_map[targets]
        loss = loss_fn(logits, target_classes)
        total_loss += float(loss.item()) * targets.numel()
        total_items += targets.numel()
        predictions = logits.argmax(dim=1)
        correct += int((predictions == target_classes).sum().item())
        top3 = logits.topk(k=min(3, logits.size(1)), dim=1).indices
        top3_correct += int((top3 == target_classes.unsqueeze(1)).any(dim=1).sum().item())
    return total_loss / max(total_items, 1), correct / max(total_items, 1), top3_correct / max(total_items, 1)


def save_checkpoint(
    path: Path,
    model: MelodyPhraseTransformer,
    optimizer: torch.optim.Optimizer,
    epoch: int,
    best_validation_loss: float,
    config: TrainConfig,
    vocab: MelodyVocabulary,
) -> None:
    torch.save(
        {
            "model_state": model.state_dict(),
            "optimizer_state": optimizer.state_dict(),
            "epoch": epoch,
            "best_validation_loss": best_validation_loss,
            "config": asdict(config),
            "vocabulary_size": len(vocab.id_to_token),
        },
        path,
    )


def load_checkpoint(path: Path) -> dict:
    return torch.load(path, map_location="cpu", weights_only=False)


def load_existing_metrics(output_dir: Path) -> list[dict[str, float | int | str]]:
    path = output_dir / "metrics.json"
    if not path.exists():
        return []
    return json.loads(path.read_text(encoding="utf-8"))


def write_json(path: Path, value: object) -> None:
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2), encoding="utf-8")


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument("--dataset", default="synthetic_melody_dataset.jsonl")
    parser.add_argument("--vocab", default="vocab.json")
    parser.add_argument("--output-dir", default="runs/melody_phrase_transformer")
    parser.add_argument("--resume")
    parser.add_argument("--reset-optimizer", action="store_true")
    parser.add_argument("--cpu", action="store_true")
    parser.add_argument("--epochs", type=int, default=30)
    parser.add_argument("--batch-size", type=int, default=64)
    parser.add_argument("--learning-rate", type=float, default=0.0003)
    parser.add_argument("--label-smoothing", type=float, default=0.02)
    parser.add_argument("--embedding-size", type=int, default=128)
    parser.add_argument("--heads", type=int, default=4)
    parser.add_argument("--layers", type=int, default=2)
    parser.add_argument("--feedforward-size", type=int, default=384)
    parser.add_argument("--dropout", type=float, default=0.1)
    parser.add_argument("--validation-ratio", type=float, default=0.15)
    parser.add_argument("--seed", type=int, default=1984)
    parser.add_argument("--save-every", type=int, default=0)
    parser.add_argument("--progress-every", type=int, default=50)
    return parser.parse_args()


if __name__ == "__main__":
    train(parse_args())
