from __future__ import annotations

import argparse
import json
from collections import Counter
from pathlib import Path


def main() -> None:
    parser = argparse.ArgumentParser(description="Validate a progression dataset against vocab.json.")
    parser.add_argument("--dataset", default="synthetic_dataset.jsonl")
    parser.add_argument("--vocab", default="vocab.json")
    args = parser.parse_args()

    vocab_data = json.loads(Path(args.vocab).read_text(encoding="utf-8"))
    allowed = set(
        vocab_data["special_tokens"]
        + vocab_data["style_tokens"]
        + vocab_data["mode_tokens"]
        + vocab_data["mood_tokens"]
        + vocab_data["progression_tokens"]
    )
    unknown: dict[str, list[int]] = {}
    styles: Counter[str] = Counter()
    modes: Counter[str] = Counter()
    moods: Counter[str] = Counter()
    examples = 0

    for line_number, line in enumerate(Path(args.dataset).read_text(encoding="utf-8").splitlines(), 1):
        if not line.strip():
            continue

        item = json.loads(line)
        examples += 1
        styles[item["style"]] += 1
        modes[item["mode"]] += 1
        moods[item["mood"]] += 1

        for token in [item["style"], item["mode"], item["mood"], *item["tokens"]]:
            if token not in allowed:
                unknown.setdefault(token, []).append(line_number)

    if unknown:
        for token, lines in unknown.items():
            print(f"unknown={token} lines={lines[:10]}")
        raise SystemExit(1)

    print(f"examples={examples}")
    print(f"styles={dict(styles)}")
    print(f"modes={dict(modes)}")
    print(f"moods={dict(moods)}")
    print("status=ok")


if __name__ == "__main__":
    main()
