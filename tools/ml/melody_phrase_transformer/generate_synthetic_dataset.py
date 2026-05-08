from __future__ import annotations

import argparse
import json
import random
from pathlib import Path


STYLES = ["STYLE_METAL", "STYLE_ROCK", "STYLE_POP", "STYLE_AMBIENT", "STYLE_BLUES"]
MODES = ["MODE_MAJOR", "MODE_NATURAL_MINOR", "MODE_DORIAN", "MODE_PHRYGIAN", "MODE_HARMONIC_MINOR"]
MOODS = ["MOOD_DARK", "MOOD_EPIC", "MOOD_BRIGHT", "MOOD_CALM", "MOOD_TENSE"]
METERS = ["METER_4_4", "METER_3_4", "METER_6_8"]
BARS = [1, 2, 4]

MODE_DEGREES = {
    "MODE_MAJOR": ["1", "2", "3", "4", "5", "6", "7", "8"],
    "MODE_NATURAL_MINOR": ["1", "2", "b3", "4", "5", "b6", "b7", "8"],
    "MODE_DORIAN": ["1", "2", "b3", "4", "5", "6", "b7", "8"],
    "MODE_PHRYGIAN": ["1", "b2", "b3", "4", "5", "b6", "b7", "8"],
    "MODE_HARMONIC_MINOR": ["1", "2", "b3", "4", "5", "b6", "7", "8"],
}

STYLE_PROGRESSIONS = {
    "STYLE_METAL": [["i", "bII"], ["i", "VI", "VII"], ["i", "VI", "bII", "VII"]],
    "STYLE_ROCK": [["I", "V", "vi", "IV"], ["i", "VII", "VI"], ["I", "bVII", "IV"]],
    "STYLE_POP": [["I", "V", "vi", "IV"], ["I", "vi", "IV", "V"], ["vi", "IV", "I", "V"]],
    "STYLE_AMBIENT": [["I", "vi", "IV"], ["i", "III", "VII"], ["i", "bVII", "IV"]],
    "STYLE_BLUES": [["I", "IV", "V"], ["i", "iv", "v"], ["I", "bVII", "IV"]],
}

MOOD_CENTER = {
    "MOOD_DARK": ["1", "b3", "4", "5", "b6", "b7"],
    "MOOD_EPIC": ["1", "4", "5", "6", "b7", "8"],
    "MOOD_BRIGHT": ["1", "2", "3", "5", "6", "8"],
    "MOOD_CALM": ["1", "2", "3", "5", "6", "b7"],
    "MOOD_TENSE": ["b2", "b3", "#4", "5", "b6", "7", "b7"],
}

DURATION_UNITS = {"1/16": 1, "1/8": 2, "1/4": 4, "1/2": 8}


def meter_units(meter: str) -> int:
    if meter == "METER_3_4":
        return 12
    if meter == "METER_6_8":
        return 12
    return 16


def choose_duration(remaining: int, style: str, rng: random.Random) -> str:
    candidates = [duration for duration, units in DURATION_UNITS.items() if units <= remaining]
    if remaining <= 2:
        return candidates[-1]
    if style == "STYLE_METAL":
        weights = [0.38, 0.42, 0.16, 0.04]
    elif style == "STYLE_AMBIENT":
        weights = [0.08, 0.22, 0.42, 0.28]
    else:
        weights = [0.18, 0.38, 0.34, 0.10]
    ordered = ["1/16", "1/8", "1/4", "1/2"]
    available = [(duration, weights[ordered.index(duration)]) for duration in candidates]
    total = sum(weight for _, weight in available)
    pick = rng.random() * total
    running = 0.0
    for duration, weight in available:
        running += weight
        if pick <= running:
            return duration
    return available[-1][0]


def choose_degree(mode: str, mood: str, previous: str | None, rng: random.Random) -> str:
    allowed = MODE_DEGREES[mode]
    preferred = [degree for degree in MOOD_CENTER[mood] if degree in allowed]
    pool = preferred * 3 + allowed
    if previous and rng.random() < 0.62:
        index = allowed.index(previous) if previous in allowed else 0
        nearby = allowed[max(0, index - 2) : min(len(allowed), index + 3)]
        pool += nearby * 2
    return rng.choice(pool)


def generate_phrase(style: str, mode: str, mood: str, meter: str, bars: int, rng: random.Random) -> list[str]:
    total_units = meter_units(meter) * bars
    remaining = total_units
    tokens = ["<BOS>"]
    previous_degree: str | None = None

    while remaining > 0:
        duration = choose_duration(remaining, style, rng)
        units = DURATION_UNITS[duration]
        rest_rate = 0.04 if style in {"STYLE_METAL", "STYLE_ROCK"} else 0.10
        rest_rate += 0.05 if mood == "MOOD_CALM" else 0.0

        if rng.random() < rest_rate and remaining > 2:
            tokens.append(f"R:{duration}")
        else:
            degree = choose_degree(mode, mood, previous_degree, rng)
            previous_degree = degree
            tokens.append(f"D:{degree}:{duration}")
        remaining -= units

    tokens.append("<EOS>")
    return tokens


def generate_dataset(output: Path, count: int, seed: int) -> None:
    rng = random.Random(seed)
    with output.open("w", encoding="utf-8") as handle:
        for _ in range(count):
            style = rng.choice(STYLES)
            mode = rng.choice(MODES)
            mood = rng.choice(MOODS)
            meter = rng.choice(METERS)
            bars = rng.choice(BARS)
            progression = rng.choice(STYLE_PROGRESSIONS[style])
            item = {
                "style": style,
                "mode": mode,
                "mood": mood,
                "meter": meter,
                "bars": bars,
                "progression": progression,
                "tokens": generate_phrase(style, mode, mood, meter, bars, rng),
            }
            handle.write(json.dumps(item, ensure_ascii=False) + "\n")


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument("--output", default="synthetic_melody_dataset.jsonl")
    parser.add_argument("--count", type=int, default=5000)
    parser.add_argument("--seed", type=int, default=1984)
    return parser.parse_args()


if __name__ == "__main__":
    args = parse_args()
    generate_dataset(Path(args.output), args.count, args.seed)
    print(f"written={args.output} examples={args.count}")
