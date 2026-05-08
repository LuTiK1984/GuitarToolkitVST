from __future__ import annotations

import argparse
import json
from collections import Counter
from pathlib import Path


DURATION_UNITS = {"1/16": 1, "1/8": 2, "1/4": 4, "1/2": 8}


def meter_units(meter: str) -> int:
    if meter == "METER_3_4":
        return 12
    if meter == "METER_6_8":
        return 12
    return 16


def token_units(token: str) -> int:
    if token in {"<BOS>", "<EOS>"}:
        return 0
    if token.startswith("R:"):
        return DURATION_UNITS[token.split(":", 1)[1]]
    if token.startswith("D:"):
        return DURATION_UNITS[token.rsplit(":", 1)[1]]
    raise ValueError(f"Unknown phrase token: {token}")


def validate(path: Path) -> None:
    styles: Counter[str] = Counter()
    modes: Counter[str] = Counter()
    moods: Counter[str] = Counter()
    meters: Counter[str] = Counter()
    examples = 0

    for line_number, line in enumerate(path.read_text(encoding="utf-8").splitlines(), start=1):
        if not line.strip():
            continue
        item = json.loads(line)
        required = {"style", "mode", "mood", "meter", "bars", "progression", "tokens"}
        missing = required.difference(item)
        if missing:
            raise RuntimeError(f"line={line_number} missing={sorted(missing)}")
        if item["tokens"][0] != "<BOS>" or item["tokens"][-1] != "<EOS>":
            raise RuntimeError(f"line={line_number} phrase must start with <BOS> and end with <EOS>")

        total = sum(token_units(token) for token in item["tokens"])
        expected = meter_units(item["meter"]) * int(item["bars"])
        if total != expected:
            raise RuntimeError(f"line={line_number} duration_units={total} expected={expected}")

        styles[item["style"]] += 1
        modes[item["mode"]] += 1
        moods[item["mood"]] += 1
        meters[item["meter"]] += 1
        examples += 1

    print(f"examples={examples}")
    print(f"styles={dict(styles)}")
    print(f"modes={dict(modes)}")
    print(f"moods={dict(moods)}")
    print(f"meters={dict(meters)}")
    print("status=ok")


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument("--dataset", default="synthetic_melody_dataset.jsonl")
    return parser.parse_args()


if __name__ == "__main__":
    validate(Path(parse_args().dataset))
