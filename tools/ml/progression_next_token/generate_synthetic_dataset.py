from __future__ import annotations

import argparse
import json
import random
from pathlib import Path


MAJOR_PROGRESSIONS = [
    ["I", "V", "vi", "IV"],
    ["I", "vi", "IV", "V"],
    ["I", "IV", "V", "I"],
    ["vi", "IV", "I", "V"],
    ["I", "V", "IV", "V"],
    ["ii", "V", "I", "I"],
    ["I", "iii", "IV", "V"],
    ["I", "IV", "vi", "V"],
    ["IV", "V", "iii", "vi"],
    ["I", "V", "vi", "iii", "IV", "I", "IV", "V"],
]

MINOR_PROGRESSIONS = [
    ["i", "VI", "VII", "i"],
    ["i", "VII", "VI", "VII"],
    ["i", "iv", "V", "i"],
    ["i", "VI", "III", "VII"],
    ["i", "VII", "VI", "V"],
    ["i", "iv", "VII", "III"],
    ["i", "bII", "i", "VII"],
    ["i", "VI", "iv", "V"],
    ["i", "v", "VI", "VII"],
    ["i", "VI", "VII", "iv", "i", "VI", "V", "i"],
]

DORIAN_PROGRESSIONS = [
    ["i", "IV", "i", "VII"],
    ["i", "ii", "III", "IV"],
    ["i", "VII", "IV", "i"],
    ["i", "v", "IV", "VII"],
]

PHRYGIAN_PROGRESSIONS = [
    ["i", "bII", "i", "VII"],
    ["i", "bII", "i", "VII"],
    ["i", "VI", "bII", "i"],
    ["i", "VII", "VI", "bII"],
]

HARMONIC_MINOR_PROGRESSIONS = [
    ["i", "iv", "V", "i"],
    ["i", "VI", "V", "i"],
    ["i", "III", "iv", "V"],
    ["i", "ii°", "V", "i"],
]

STYLE_WEIGHTS = {
    "STYLE_METAL": {
        "MODE_NATURAL_MINOR": MINOR_PROGRESSIONS,
        "MODE_PHRYGIAN": PHRYGIAN_PROGRESSIONS,
        "MODE_HARMONIC_MINOR": HARMONIC_MINOR_PROGRESSIONS,
    },
    "STYLE_ROCK": {
        "MODE_MAJOR": MAJOR_PROGRESSIONS,
        "MODE_NATURAL_MINOR": MINOR_PROGRESSIONS,
        "MODE_DORIAN": DORIAN_PROGRESSIONS,
    },
    "STYLE_POP": {
        "MODE_MAJOR": MAJOR_PROGRESSIONS,
        "MODE_NATURAL_MINOR": MINOR_PROGRESSIONS,
    },
    "STYLE_AMBIENT": {
        "MODE_MAJOR": MAJOR_PROGRESSIONS,
        "MODE_DORIAN": DORIAN_PROGRESSIONS,
        "MODE_NATURAL_MINOR": MINOR_PROGRESSIONS,
    },
    "STYLE_BLUES": {
        "MODE_MAJOR": [["I", "IV", "I", "V"], ["I", "IV", "V", "IV"], ["I", "I", "IV", "V"]],
        "MODE_DORIAN": DORIAN_PROGRESSIONS,
    },
}

MOODS = ["MOOD_DARK", "MOOD_EPIC", "MOOD_BRIGHT", "MOOD_CALM", "MOOD_TENSE"]


def rotate(tokens: list[str], shift: int) -> list[str]:
    shift %= len(tokens)
    return tokens[shift:] + tokens[:shift]


def stretch(tokens: list[str], target_length: int) -> list[str]:
    result: list[str] = []
    while len(result) < target_length:
        result.extend(tokens)
    return result[:target_length]


def mutate(tokens: list[str], mood: str, rng: random.Random) -> list[str]:
    result = list(tokens)
    if mood == "MOOD_TENSE" and "V" in result and rng.random() < 0.35:
        result.insert(max(1, len(result) - 1), "bII")
    if mood == "MOOD_EPIC" and "VI" in result and rng.random() < 0.30:
        result.append("VII")
    if mood == "MOOD_CALM" and len(result) > 4 and rng.random() < 0.45:
        result = result[:4]
    return result


def build_examples(count: int, seed: int) -> list[dict[str, object]]:
    rng = random.Random(seed)
    examples: list[dict[str, object]] = []

    while len(examples) < count:
        style = rng.choice(list(STYLE_WEIGHTS.keys()))
        mode = rng.choice(list(STYLE_WEIGHTS[style].keys()))
        mood = rng.choice(MOODS)
        base = rng.choice(STYLE_WEIGHTS[style][mode])
        length = rng.choice([4, 4, 4, 6, 8])
        tokens = stretch(rotate(base, rng.randrange(len(base))), length)
        tokens = mutate(tokens, mood, rng)
        examples.append(
            {
                "style": style,
                "mode": mode,
                "mood": mood,
                "tokens": ["<BOS>", *tokens, "<EOS>"],
            }
        )

    return examples


def main() -> None:
    parser = argparse.ArgumentParser(description="Generate a license-clean synthetic progression dataset.")
    parser.add_argument("--output", default="synthetic_dataset.jsonl")
    parser.add_argument("--count", type=int, default=5000)
    parser.add_argument("--seed", type=int, default=1984)
    args = parser.parse_args()

    examples = build_examples(args.count, args.seed)
    output = Path(args.output)
    output.write_text(
        "\n".join(json.dumps(example, ensure_ascii=False, separators=(",", ":")) for example in examples) + "\n",
        encoding="utf-8",
    )
    print(f"written={output} examples={len(examples)}")


if __name__ == "__main__":
    main()
