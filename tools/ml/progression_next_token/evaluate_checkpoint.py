from __future__ import annotations

import argparse
import json
import math
from pathlib import Path

import torch

from model import ProgressionNextTokenModel, Vocabulary


def add_group_metric(groups: dict[str, dict[str, list[float] | set[str]]], key: str, entropy: float, top3_mass: float, top1: str) -> None:
    bucket = groups.setdefault(key, {"entropy": [], "top3_mass": [], "top1": set()})
    bucket["entropy"].append(entropy)
    bucket["top3_mass"].append(top3_mass)
    bucket["top1"].add(top1)


def summarize_groups(groups: dict[str, dict[str, list[float] | set[str]]]) -> dict[str, dict[str, float | int]]:
    summary: dict[str, dict[str, float | int]] = {}
    for key, values in sorted(groups.items()):
        entropy_values = values["entropy"]
        top3_values = values["top3_mass"]
        top1_values = values["top1"]
        assert isinstance(entropy_values, list)
        assert isinstance(top3_values, list)
        assert isinstance(top1_values, set)
        summary[key] = {
            "count": len(entropy_values),
            "avg_entropy": round(sum(entropy_values) / max(len(entropy_values), 1), 4),
            "avg_top3_mass": round(sum(top3_values) / max(len(top3_values), 1), 4),
            "distinct_top1": len(top1_values),
        }

    return summary


@torch.no_grad()
def evaluate(args: argparse.Namespace) -> None:
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

    prompts = [json.loads(line) for line in Path(args.prompts).read_text(encoding="utf-8").splitlines() if line.strip()]
    output_mask = torch.full((len(vocab.id_to_token),), -1_000_000.0, dtype=torch.float32)
    output_mask[vocab.output_token_ids] = 0.0

    rows = []
    top1_tokens: list[str] = []
    entropy_values: list[float] = []
    top3_mass_values: list[float] = []
    by_style: dict[str, dict[str, list[float] | set[str]]] = {}
    by_mode: dict[str, dict[str, list[float] | set[str]]] = {}
    by_mood: dict[str, dict[str, list[float] | set[str]]] = {}

    for prompt in prompts:
        previous = [vocab.encode(token) for token in prompt["previous"]]
        logits = model(
            torch.tensor([vocab.encode(prompt["style"])], dtype=torch.long),
            torch.tensor([vocab.encode(prompt["mode"])], dtype=torch.long),
            torch.tensor([vocab.encode(prompt["mood"])], dtype=torch.long),
            torch.tensor([previous], dtype=torch.long),
        )
        probabilities = torch.softmax(logits + output_mask.unsqueeze(0), dim=1)[0]
        top = probabilities.topk(k=args.top_k)
        predictions = [
            {
                "token": vocab.id_to_token[int(index)],
                "probability": round(float(probability), 4),
            }
            for probability, index in zip(top.values, top.indices)
        ]

        entropy = -sum(float(prob) * math.log(max(float(prob), 1e-12)) for prob in probabilities if float(prob) > 0)
        top3_mass = float(top.values[: min(3, len(top.values))].sum().item())
        top1_token = predictions[0]["token"]
        top1_tokens.append(top1_token)
        entropy_values.append(entropy)
        top3_mass_values.append(top3_mass)
        add_group_metric(by_style, prompt["style"], entropy, top3_mass, top1_token)
        add_group_metric(by_mode, prompt["mode"], entropy, top3_mass, top1_token)
        add_group_metric(by_mood, prompt["mood"], entropy, top3_mass, top1_token)

        rows.append(
            {
                "name": prompt["name"],
                "context": {
                    "style": prompt["style"],
                    "mode": prompt["mode"],
                    "mood": prompt["mood"],
                    "previous": prompt["previous"],
                },
                "entropy": round(entropy, 4),
                "top3_mass": round(top3_mass, 4),
                "predictions": predictions,
            }
        )

    summary = {
        "checkpoint": args.checkpoint,
        "prompt_count": len(rows),
        "avg_entropy": round(sum(entropy_values) / max(len(entropy_values), 1), 4),
        "avg_top3_mass": round(sum(top3_mass_values) / max(len(top3_mass_values), 1), 4),
        "distinct_top1": len(set(top1_tokens)),
        "top1_tokens": top1_tokens,
        "by_style": summarize_groups(by_style),
        "by_mode": summarize_groups(by_mode),
        "by_mood": summarize_groups(by_mood),
    }

    result = {"summary": summary, "prompts": rows}
    print(json.dumps(result, ensure_ascii=False, indent=2))


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Evaluate checkpoint diversity on fixed musical prompts.")
    parser.add_argument("--checkpoint", default="runs/progression_next_token/best_model.pt")
    parser.add_argument("--vocab", default="vocab.json")
    parser.add_argument("--prompts", default="eval_prompts.jsonl")
    parser.add_argument("--top-k", type=int, default=8)
    return parser.parse_args()


if __name__ == "__main__":
    evaluate(parse_args())
