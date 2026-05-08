from __future__ import annotations

import argparse
from pathlib import Path

import torch

from model import MelodyPhraseTransformer, MelodyVocabulary
from train import TrainConfig, load_checkpoint


def export(args: argparse.Namespace) -> None:
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

    output = Path(args.output)
    output.parent.mkdir(parents=True, exist_ok=True)
    context_tokens = torch.tensor([[vocab.encode("STYLE_METAL"), vocab.encode("MODE_NATURAL_MINOR"), vocab.encode("MOOD_DARK"), vocab.encode("METER_4_4"), vocab.encode("BARS_2"), vocab.encode("i")]], dtype=torch.long)
    previous_tokens = torch.tensor([[vocab.encode("<BOS>"), vocab.encode("D:1:1/8")]], dtype=torch.long)

    torch.onnx.export(
        model,
        (context_tokens, previous_tokens),
        output,
        input_names=["context_tokens", "previous_tokens"],
        output_names=["next_token_logits"],
        dynamic_axes={
            "context_tokens": {1: "context_length"},
            "previous_tokens": {1: "sequence_length"},
            "next_token_logits": {1: "vocabulary_size"},
        },
        opset_version=17,
        dynamo=False,
    )
    print(f"exported={output}")


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument("--checkpoint", default="runs/melody_phrase_transformer/best_model.pt")
    parser.add_argument("--vocab", default="vocab.json")
    parser.add_argument("--output", default="runs/melody_phrase_transformer/MelodyPhraseTransformer.onnx")
    return parser.parse_args()


if __name__ == "__main__":
    export(parse_args())
