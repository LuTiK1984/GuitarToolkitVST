from __future__ import annotations

import argparse
import json
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
    config = TrainConfig(
        embedding_size=args.embedding_size,
        hidden_size=args.hidden_size,
        layers=args.layers,
        dropout=args.dropout,
        rnn_type=args.rnn_type,
        epochs=args.epochs,
        batch_size=args.batch_size,
        learning_rate=args.learning_rate,
        max_sequence_length=args.max_sequence_length,
        validation_ratio=args.validation_ratio,
        seed=args.seed,
    )
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
    model = ProgressionNextTokenModel(
        vocabulary_size=len(vocab.id_to_token),
        embedding_size=config.embedding_size,
        hidden_size=config.hidden_size,
        layers=config.layers,
        dropout=config.dropout,
        rnn_type=config.rnn_type,
    ).to(device)

    optimizer = torch.optim.AdamW(model.parameters(), lr=config.learning_rate)
    loss_fn = nn.CrossEntropyLoss(ignore_index=vocab.pad_id)
    output_mask = build_output_mask(len(vocab.id_to_token), vocab.output_token_ids, device)
    metrics: list[dict[str, float | int]] = []
    best_validation_loss = float("inf")

    for epoch in range(1, config.epochs + 1):
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
            logits = apply_output_mask(model(style_id, mode_id, mood_id, previous_tokens), output_mask)
            loss = loss_fn(logits, targets)
            loss.backward()
            optimizer.step()

            total_loss += float(loss.item()) * targets.numel()
            total_items += targets.numel()

        train_loss = total_loss / max(total_items, 1)
        validation_loss, accuracy, top3_accuracy = evaluate(model, validation_loader, loss_fn, device, output_mask)
        metrics.append(
            {
                "epoch": epoch,
                "train_loss": train_loss,
                "validation_loss": validation_loss,
                "accuracy": accuracy,
                "top3_accuracy": top3_accuracy,
            }
        )
        best_validation_loss = min(best_validation_loss, validation_loss)

        print(
            f"epoch={epoch:03d} "
            f"train_loss={train_loss:.4f} "
            f"val_loss={validation_loss:.4f} "
            f"acc={accuracy:.3f} "
            f"top3={top3_accuracy:.3f}"
        )

    output_dir = Path(args.output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)
    torch.save(
        {
            "model_state": model.state_dict(),
            "config": asdict(config),
            "vocabulary_size": len(vocab.id_to_token),
        },
        output_dir / "ProgressionNextTokenModel.pt",
    )
    (output_dir / "training_config.json").write_text(
        json.dumps(asdict(config), indent=2, ensure_ascii=False),
        encoding="utf-8",
    )
    (output_dir / "metrics.json").write_text(
        json.dumps(
            {
                "best_validation_loss": best_validation_loss,
                "last": metrics[-1],
                "epochs": metrics,
            },
            indent=2,
            ensure_ascii=False,
        ),
        encoding="utf-8",
    )
    print(f"saved={output_dir / 'ProgressionNextTokenModel.pt'}")


@torch.no_grad()
def evaluate(
    model: ProgressionNextTokenModel,
    loader: DataLoader,
    loss_fn: nn.CrossEntropyLoss,
    device: torch.device,
    output_mask: torch.Tensor,
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

        logits = apply_output_mask(model(style_id, mode_id, mood_id, previous_tokens), output_mask)
        loss = loss_fn(logits, targets)
        total_loss += float(loss.item()) * targets.numel()
        total_items += targets.numel()

        predictions = logits.argmax(dim=1)
        correct += int((predictions == targets).sum().item())
        top3 = logits.topk(k=min(3, logits.shape[1]), dim=1).indices
        top3_correct += int((top3 == targets.unsqueeze(1)).any(dim=1).sum().item())

    total_items = max(total_items, 1)
    return total_loss / total_items, correct / total_items, top3_correct / total_items


def build_output_mask(vocabulary_size: int, output_token_ids: list[int], device: torch.device) -> torch.Tensor:
    mask = torch.full((vocabulary_size,), -1_000_000.0, dtype=torch.float32, device=device)
    mask[output_token_ids] = 0.0
    return mask


def apply_output_mask(logits: torch.Tensor, output_mask: torch.Tensor) -> torch.Tensor:
    return logits + output_mask.unsqueeze(0)


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
    parser.add_argument("--max-sequence-length", type=int, default=16)
    parser.add_argument("--validation-ratio", type=float, default=0.15)
    parser.add_argument("--seed", type=int, default=1984)
    parser.add_argument("--cpu", action="store_true")
    return parser.parse_args()


if __name__ == "__main__":
    train(parse_args())
