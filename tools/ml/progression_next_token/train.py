from __future__ import annotations

import argparse
import json
from dataclasses import asdict, dataclass
from pathlib import Path

import torch
from torch import nn
from torch.utils.data import DataLoader, Dataset

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
    )
    vocab = Vocabulary.load(args.vocab)
    dataset = ProgressionDataset(Path(args.dataset), vocab, config.max_sequence_length)
    if len(dataset) == 0:
        raise RuntimeError("Dataset is empty.")

    loader = DataLoader(
        dataset,
        batch_size=config.batch_size,
        shuffle=True,
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

    model.train()
    for epoch in range(1, config.epochs + 1):
        total_loss = 0.0
        total_items = 0

        for style_id, mode_id, mood_id, previous_tokens, targets in loader:
            style_id = style_id.to(device)
            mode_id = mode_id.to(device)
            mood_id = mood_id.to(device)
            previous_tokens = previous_tokens.to(device)
            targets = targets.to(device)

            optimizer.zero_grad(set_to_none=True)
            logits = model(style_id, mode_id, mood_id, previous_tokens)
            loss = loss_fn(logits, targets)
            loss.backward()
            optimizer.step()

            total_loss += float(loss.item()) * targets.numel()
            total_items += targets.numel()

        print(f"epoch={epoch:03d} loss={total_loss / max(total_items, 1):.4f}")

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
    print(f"saved={output_dir / 'ProgressionNextTokenModel.pt'}")


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
    parser.add_argument("--cpu", action="store_true")
    return parser.parse_args()


if __name__ == "__main__":
    train(parse_args())
