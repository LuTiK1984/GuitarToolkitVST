from __future__ import annotations

import argparse
import json
from pathlib import Path

import torch

from model import MelodyPhraseTransformer, MelodyVocabulary
from train import TrainConfig, load_checkpoint


def inspect(args: argparse.Namespace) -> None:
    vocab = MelodyVocabulary.load(args.vocab)
    checkpoint = load_checkpoint(Path(args.checkpoint))
    config = TrainConfig(**checkpoint["config"])
    model = MelodyPhraseTransformer(
        vocabulary_size=len(vocab.id_to_token),
        embedding_size=config.embedding_size,
        heads=config.heads,
        layers=config.layers,
        feedforward_size=config.feedforward_size,
        dropout=0.0,
        max_sequence_length=config.max_sequence_length + config.max_context_length,
    )
    model.load_state_dict(checkpoint["model_state"])
    model.eval()

    context = [
        args.style,
        args.mode,
        args.mood,
        args.meter,
        f"BARS_{args.bars}",
        *split_csv(args.progression),
    ][: config.max_context_length]
    previous = split_csv(args.previous)
    context_ids = torch.tensor([[vocab.encode(token) for token in context]], dtype=torch.long)
    previous_ids = torch.tensor([[vocab.encode(token) for token in previous]], dtype=torch.long)

    with torch.no_grad():
        logits = model(context_ids, previous_ids)
        output_ids = torch.tensor(vocab.output_token_ids, dtype=torch.long)
        probabilities = logits.index_select(1, output_ids).softmax(dim=1)[0]
        top = probabilities.topk(k=min(args.top, probabilities.numel()))

    result = [
        {
            "token": vocab.id_to_token[vocab.output_token_ids[int(index)]],
            "probability": round(float(probability), 4),
        }
        for probability, index in zip(top.values, top.indices)
    ]
    print(json.dumps(result, ensure_ascii=False, indent=2))


def split_csv(value: str) -> list[str]:
    return [part.strip() for part in value.split(",") if part.strip()]


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument("--checkpoint", default="runs/melody_phrase_transformer/best_model.pt")
    parser.add_argument("--vocab", default="vocab.json")
    parser.add_argument("--style", default="STYLE_METAL")
    parser.add_argument("--mode", default="MODE_NATURAL_MINOR")
    parser.add_argument("--mood", default="MOOD_DARK")
    parser.add_argument("--meter", default="METER_4_4")
    parser.add_argument("--bars", type=int, default=2)
    parser.add_argument("--progression", default="i,VI")
    parser.add_argument("--previous", default="<BOS>,D:1:1/8,D:b3:1/8")
    parser.add_argument("--top", type=int, default=8)
    return parser.parse_args()


if __name__ == "__main__":
    inspect(parse_args())
