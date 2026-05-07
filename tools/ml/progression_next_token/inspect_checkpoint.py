from __future__ import annotations

import argparse
import json
from pathlib import Path

import torch

from model import ProgressionNextTokenModel, Vocabulary


@torch.no_grad()
def inspect(args: argparse.Namespace) -> None:
    checkpoint = torch.load(args.checkpoint, map_location="cpu", weights_only=False)
    config = checkpoint["config"]
    vocab = Vocabulary.load(args.vocab)
    model = ProgressionNextTokenModel(
        vocabulary_size=checkpoint["vocabulary_size"],
        embedding_size=config["embedding_size"],
        hidden_size=config["hidden_size"],
        layers=config["layers"],
        dropout=config["dropout"],
        rnn_type=config["rnn_type"],
    )
    model.load_state_dict(checkpoint["model_state"])
    model.eval()

    previous = [vocab.encode(token.strip()) for token in args.previous.split(",") if token.strip()]
    if not previous:
        previous = [vocab.encode("<BOS>")]

    logits = model(
        torch.tensor([vocab.encode(args.style)], dtype=torch.long),
        torch.tensor([vocab.encode(args.mode)], dtype=torch.long),
        torch.tensor([vocab.encode(args.mood)], dtype=torch.long),
        torch.tensor([previous], dtype=torch.long),
    )
    mask = torch.full((len(vocab.id_to_token),), -1_000_000.0, dtype=torch.float32)
    mask[vocab.output_token_ids] = 0.0
    probabilities = torch.softmax(logits + mask.unsqueeze(0), dim=1)[0]
    top = probabilities.topk(k=args.top_k)
    result = [
        {"token": vocab.id_to_token[int(index)], "probability": round(float(probability), 4)}
        for probability, index in zip(top.values, top.indices)
    ]
    print(json.dumps(result, ensure_ascii=False, indent=2))


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Inspect top-k next-token predictions from a checkpoint.")
    parser.add_argument("--checkpoint", default="runs/progression_next_token/ProgressionNextTokenModel.pt")
    parser.add_argument("--vocab", default="vocab.json")
    parser.add_argument("--style", default="STYLE_METAL")
    parser.add_argument("--mode", default="MODE_NATURAL_MINOR")
    parser.add_argument("--mood", default="MOOD_DARK")
    parser.add_argument("--previous", default="<BOS>,i,VI")
    parser.add_argument("--top-k", type=int, default=8)
    return parser.parse_args()


if __name__ == "__main__":
    inspect(parse_args())
