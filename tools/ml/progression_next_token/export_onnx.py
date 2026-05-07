from __future__ import annotations

import argparse
from pathlib import Path

import torch

from model import ProgressionNextTokenModel, Vocabulary


def export(args: argparse.Namespace) -> None:
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

    style_id = torch.tensor([vocab.encode("STYLE_METAL")], dtype=torch.long)
    mode_id = torch.tensor([vocab.encode("MODE_NATURAL_MINOR")], dtype=torch.long)
    mood_id = torch.tensor([vocab.encode("MOOD_DARK")], dtype=torch.long)
    previous_tokens = torch.tensor(
        [[vocab.encode("<BOS>"), vocab.encode("i"), vocab.encode("VI")]],
        dtype=torch.long,
    )

    output_path = Path(args.output)
    output_path.parent.mkdir(parents=True, exist_ok=True)

    torch.onnx.export(
        model,
        (style_id, mode_id, mood_id, previous_tokens),
        output_path,
        input_names=["style_id", "mode_id", "mood_id", "previous_tokens"],
        output_names=["next_token_logits"],
        dynamic_axes={
            "previous_tokens": {1: "sequence_length"}
        },
        opset_version=17,
        dynamo=False,
    )
    print(f"exported={output_path}")


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Export ProgressionNextTokenModel checkpoint to ONNX.")
    parser.add_argument("--checkpoint", default="runs/progression_next_token/ProgressionNextTokenModel.pt")
    parser.add_argument("--vocab", default="vocab.json")
    parser.add_argument("--output", default="runs/progression_next_token/ProgressionNextTokenModel.onnx")
    return parser.parse_args()


if __name__ == "__main__":
    export(parse_args())
