from __future__ import annotations

import argparse
import json
import shutil
from dataclasses import asdict, dataclass
from pathlib import Path

import torch
from torch import nn
from torch.utils.data import DataLoader, Dataset, random_split

from model import ProgressionNextTokenModel, Vocabulary


@dataclass(frozen=True)
class TrainConfig:
    embedding_size: int = 96
    hidden_size: int = 192
    layers: int = 1
    dropout: float = 0.0
    rnn_type: str = "gru"
    epochs: int = 40
    batch_size: int = 16
    learning_rate: float = 0.001
    label_smoothing: float = 0.0
    max_sequence_length: int = 16
    validation_ratio: float = 0.15
    seed: int = 1984


class ProgressionDataset(Dataset):
    def __init__(self, dataset_path: Path, vocab: Vocabulary, max_sequence_length: int) -> None:
        self.vocab = vocab
        self.max_sequence_length = max_sequence_length
        self.examples: list[tuple[int, int, int, list[int], int]] = []

        for line in dataset_path.read_text(encoding="utf-8").splitlines():
            if not line.strip():
                continue

            item = json.loads(line)
            style_id = vocab.encode(item["style"])
            mode_id = vocab.encode(item["mode"])
            mood_id = vocab.encode(item["mood"])
            tokens = [vocab.encode(token) for token in item["tokens"]]

            for index in range(1, len(tokens)):
                previous = tokens[:index][-max_sequence_length:]
                target = tokens[index]
                self.examples.append((style_id, mode_id, mood_id, previous, target))

    def __len__(self) -> int:
        return len(self.examples)

    def __getitem__(self, index: int) -> tuple[int, int, int, list[int], int]:
        return self.examples[index]


def collate_batch(batch: list[tuple[int, int, int, list[int], int]], pad_id: int) -> tuple[torch.Tensor, ...]:
    max_len = max(len(item[3]) for item in batch)
    style_ids: list[int] = []
    mode_ids: list[int] = []
    mood_ids: list[int] = []
    previous_tokens: list[list[int]] = []
    targets: list[int] = []

    for style_id, mode_id, mood_id, previous, target in batch:
        style_ids.append(style_id)
        mode_ids.append(mode_id)
        mood_ids.append(mood_id)
        previous_tokens.append(previous + [pad_id] * (max_len - len(previous)))
        targets.append(target)

    return (
        torch.tensor(style_ids, dtype=torch.long),
        torch.tensor(mode_ids, dtype=torch.long),
        torch.tensor(mood_ids, dtype=torch.long),
        torch.tensor(previous_tokens, dtype=torch.long),
        torch.tensor(targets, dtype=torch.long),
    )


def train(args: argparse.Namespace) -> None:
    resume_checkpoint = Path(args.resume) if args.resume else None
    checkpoint = load_checkpoint(resume_checkpoint) if resume_checkpoint else None
    checkpoint_config = checkpoint.get("config") if checkpoint else None

    config = TrainConfig(
        embedding_size=args.embedding_size,
        hidden_size=args.hidden_size,
        layers=args.layers,
        dropout=args.dropout,
        rnn_type=args.rnn_type,
        epochs=args.epochs,
        batch_size=args.batch_size,
        learning_rate=args.learning_rate,
        label_smoothing=args.label_smoothing,
        max_sequence_length=args.max_sequence_length,
        validation_ratio=args.validation_ratio,
        seed=args.seed,
    )
    if checkpoint_config:
        resumed_config = dict(checkpoint_config)
        resumed_config["epochs"] = args.epochs
        resumed_config["batch_size"] = args.batch_size
        resumed_config["learning_rate"] = args.learning_rate
        resumed_config["label_smoothing"] = args.label_smoothing
        resumed_config["validation_ratio"] = args.validation_ratio
        resumed_config["seed"] = args.seed
        config = TrainConfig(**resumed_config)

    torch.manual_seed(config.seed)
    vocab = Vocabulary.load(args.vocab)
    dataset = ProgressionDataset(Path(args.dataset), vocab, config.max_sequence_length)
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
    model = ProgressionNextTokenModel(
        vocabulary_size=len(vocab.id_to_token),
        embedding_size=config.embedding_size,
        hidden_size=config.hidden_size,
        layers=config.layers,
        dropout=config.dropout,
        rnn_type=config.rnn_type,
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
        print(f"resumed={resume_checkpoint} start_epoch={start_epoch} best_val_loss={best_validation_loss:.4f}")

    loss_fn = nn.CrossEntropyLoss(ignore_index=-1, label_smoothing=config.label_smoothing)
    output_token_ids = torch.tensor(vocab.output_token_ids, dtype=torch.long, device=device)
    output_index_map = build_output_index_map(len(vocab.id_to_token), vocab.output_token_ids, device)

    output_dir = Path(args.output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)
    total_target_epoch = start_epoch + config.epochs

    for local_epoch in range(1, config.epochs + 1):
        epoch = start_epoch + local_epoch
        model.train()
        total_loss = 0.0
        total_items = 0

        for style_id, mode_id, mood_id, previous_tokens, targets in train_loader:
            style_id = style_id.to(device)
            mode_id = mode_id.to(device)
            mood_id = mood_id.to(device)
            previous_tokens = previous_tokens.to(device)
            targets = targets.to(device)

            optimizer.zero_grad(set_to_none=True)
            logits = model(style_id, mode_id, mood_id, previous_tokens).index_select(1, output_token_ids)
            target_classes = output_index_map[targets]
            loss = loss_fn(logits, target_classes)
            loss.backward()
            optimizer.step()

            total_loss += float(loss.item()) * targets.numel()
            total_items += targets.numel()

        train_loss = total_loss / max(total_items, 1)
        validation_loss, accuracy, top3_accuracy = evaluate(model, validation_loader, loss_fn, device, output_token_ids, output_index_map)
        improved = validation_loss < best_validation_loss
        if improved:
            best_validation_loss = validation_loss

        metrics.append(
            {
                "epoch": epoch,
                "local_epoch": local_epoch,
                "train_loss": train_loss,
                "validation_loss": validation_loss,
                "accuracy": accuracy,
                "top3_accuracy": top3_accuracy,
                "device": str(device),
                "mode": "resume" if checkpoint else "fresh",
            }
        )

        print(
            f"epoch={epoch:03d}/{total_target_epoch:03d} "
            f"train_loss={train_loss:.4f} "
            f"val_loss={validation_loss:.4f} "
            f"acc={accuracy:.3f} "
            f"top3={top3_accuracy:.3f} "
            f"{'best' if improved else ''}"
        )

        checkpoint_data = build_checkpoint(
            model,
            optimizer,
            config,
            vocabulary_size=len(vocab.id_to_token),
            epoch=epoch,
            best_validation_loss=best_validation_loss,
        )
        latest_path = output_dir / "ProgressionNextTokenModel.pt"
        torch.save(checkpoint_data, latest_path)
        if improved:
            torch.save(checkpoint_data, output_dir / "best_model.pt")
        if args.save_every > 0 and epoch % args.save_every == 0:
            torch.save(checkpoint_data, output_dir / f"checkpoint_epoch_{epoch:04d}.pt")

    latest_path = output_dir / "ProgressionNextTokenModel.pt"
    if not (output_dir / "best_model.pt").exists():
        shutil.copy2(latest_path, output_dir / "best_model.pt")

    write_training_files(
        output_dir,
        config,
        metrics,
        best_validation_loss,
        latest_path,
        output_dir / "best_model.pt",
    )
    print(f"saved={latest_path}")
    print(f"best={output_dir / 'best_model.pt'}")


def load_checkpoint(path: Path | None) -> dict[str, object] | None:
    if path is None:
        return None
    if not path.exists():
        raise FileNotFoundError(path)
    return torch.load(path, map_location="cpu", weights_only=False)


def load_existing_metrics(output_dir: Path) -> list[dict[str, float | int | str]]:
    metrics_path = output_dir / "metrics.json"
    if not metrics_path.exists():
        return []

    data = json.loads(metrics_path.read_text(encoding="utf-8"))
    return data.get("epochs", [])


def build_checkpoint(
    model: ProgressionNextTokenModel,
    optimizer: torch.optim.Optimizer,
    config: TrainConfig,
    vocabulary_size: int,
    epoch: int,
    best_validation_loss: float,
) -> dict[str, object]:
    return {
        "model_state": model.state_dict(),
        "optimizer_state": optimizer.state_dict(),
        "config": asdict(config),
        "vocabulary_size": vocabulary_size,
        "epoch": epoch,
        "best_validation_loss": best_validation_loss,
    }


def write_training_files(
    output_dir: Path,
    config: TrainConfig,
    metrics: list[dict[str, float | int | str]],
    best_validation_loss: float,
    latest_path: Path,
    best_path: Path,
) -> None:
    (output_dir / "training_config.json").write_text(
        json.dumps(asdict(config), indent=2, ensure_ascii=False),
        encoding="utf-8",
    )
    (output_dir / "metrics.json").write_text(
        json.dumps(
            {
                "best_validation_loss": best_validation_loss,
                "last": metrics[-1],
                "latest_checkpoint": str(latest_path),
                "best_checkpoint": str(best_path),
                "epochs": metrics,
            },
            indent=2,
            ensure_ascii=False,
        ),
        encoding="utf-8",
    )


@torch.no_grad()
def evaluate(
    model: ProgressionNextTokenModel,
    loader: DataLoader,
    loss_fn: nn.CrossEntropyLoss,
    device: torch.device,
    output_token_ids: torch.Tensor,
    output_index_map: torch.Tensor,
) -> tuple[float, float, float]:
    model.eval()
    total_loss = 0.0
    total_items = 0
    correct = 0
    top3_correct = 0

    for style_id, mode_id, mood_id, previous_tokens, targets in loader:
        style_id = style_id.to(device)
        mode_id = mode_id.to(device)
        mood_id = mood_id.to(device)
        previous_tokens = previous_tokens.to(device)
        targets = targets.to(device)

        logits = model(style_id, mode_id, mood_id, previous_tokens).index_select(1, output_token_ids)
        target_classes = output_index_map[targets]
        loss = loss_fn(logits, target_classes)
        total_loss += float(loss.item()) * targets.numel()
        total_items += targets.numel()

        predictions = logits.argmax(dim=1)
        correct += int((predictions == target_classes).sum().item())
        top3 = logits.topk(k=min(3, logits.shape[1]), dim=1).indices
        top3_correct += int((top3 == target_classes.unsqueeze(1)).any(dim=1).sum().item())

    total_items = max(total_items, 1)
    return total_loss / total_items, correct / total_items, top3_correct / total_items


def build_output_index_map(vocabulary_size: int, output_token_ids: list[int], device: torch.device) -> torch.Tensor:
    output_index_map = torch.full((vocabulary_size,), -1, dtype=torch.long, device=device)
    output_index_map[torch.tensor(output_token_ids, dtype=torch.long, device=device)] = torch.arange(
        len(output_token_ids),
        dtype=torch.long,
        device=device,
    )
    return output_index_map


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Train the first GuitarToolkit progression next-token model.")
    parser.add_argument("--dataset", default="sample_dataset.jsonl")
    parser.add_argument("--vocab", default="vocab.json")
    parser.add_argument("--output-dir", default="runs/progression_next_token")
    parser.add_argument("--embedding-size", type=int, default=96)
    parser.add_argument("--hidden-size", type=int, default=192)
    parser.add_argument("--layers", type=int, default=1)
    parser.add_argument("--dropout", type=float, default=0.0)
    parser.add_argument("--rnn-type", choices=["gru", "lstm"], default="gru")
    parser.add_argument("--epochs", type=int, default=40)
    parser.add_argument("--batch-size", type=int, default=16)
    parser.add_argument("--learning-rate", type=float, default=0.001)
    parser.add_argument("--label-smoothing", type=float, default=0.0, help="Softens one-hot targets so the model can keep plausible alternatives alive.")
    parser.add_argument("--max-sequence-length", type=int, default=16)
    parser.add_argument("--validation-ratio", type=float, default=0.15)
    parser.add_argument("--seed", type=int, default=1984)
    parser.add_argument("--resume", default=None, help="Path to an existing .pt checkpoint to continue training.")
    parser.add_argument("--reset-optimizer", action="store_true", help="Resume weights but start with a fresh optimizer.")
    parser.add_argument("--save-every", type=int, default=0, help="Save numbered checkpoints every N global epochs.")
    parser.add_argument("--cpu", action="store_true")
    return parser.parse_args()


if __name__ == "__main__":
    train(parse_args())
