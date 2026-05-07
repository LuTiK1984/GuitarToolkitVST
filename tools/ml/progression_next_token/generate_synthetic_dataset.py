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
    ["I", "IV", "I", "V"],
    ["I", "ii", "V", "I"],
    ["I", "vi", "ii", "V"],
    ["IV", "I", "V", "vi"],
    ["vi", "V", "IV", "I"],
    ["I", "V", "iii", "IV"],
    ["I", "IV", "V", "vi"],
    ["ii", "IV", "V", "I"],
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
    ["i", "VI", "V", "i"],
    ["i", "VI", "bII", "V"],
    ["i", "VI", "VII", "III"],
    ["i", "III", "VII", "VI"],
    ["i", "iv", "VI", "VII"],
    ["i", "v", "iv", "VII"],
    ["i", "VII", "III", "VI"],
    ["i", "bVI", "bVII", "i"],
    ["i", "v", "VI", "VII"],
    ["i", "VI", "VII", "iv", "i", "VI", "V", "i"],
]

DORIAN_PROGRESSIONS = [
    ["i", "IV", "i", "VII"],
    ["i", "ii", "III", "IV"],
    ["i", "VII", "IV", "i"],
    ["i", "v", "IV", "VII"],
    ["i", "IV", "VII", "i"],
    ["i", "ii", "IV", "i"],
    ["i", "v", "VII", "IV"],
    ["i", "III", "IV", "v"],
]

PHRYGIAN_PROGRESSIONS = [
    ["i", "bII", "i", "VII"],
    ["i", "VI", "bII", "i"],
    ["i", "VII", "VI", "bII"],
    ["i", "bII", "VII", "i"],
    ["i", "VII", "bII", "i"],
    ["i", "bII", "VI", "VII"],
    ["i", "VI", "VII", "bII"],
]

HARMONIC_MINOR_PROGRESSIONS = [
    ["i", "iv", "V", "i"],
    ["i", "VI", "V", "i"],
    ["i", "III", "iv", "V"],
    ["i", "ii°", "V", "i"],
    ["i", "VI", "ii°", "V"],
    ["i", "iv", "VI", "V"],
    ["i", "III", "VI", "V"],
    ["i", "V", "VI", "V"],
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
        "MODE_MAJOR": [
            ["I", "IV", "I", "V"],
            ["I", "IV", "V", "IV"],
            ["I", "I", "IV", "V"],
            ["I", "IV", "I", "I", "IV", "IV", "I", "I", "V", "IV", "I", "V"],
        ],
        "MODE_DORIAN": DORIAN_PROGRESSIONS,
    },
}

MOODS = ["MOOD_DARK", "MOOD_EPIC", "MOOD_BRIGHT", "MOOD_CALM", "MOOD_TENSE"]
STYLE_MODE_PAIRS = [(style, mode) for style, modes in STYLE_WEIGHTS.items() for mode in modes]

CADENCES = {
    "MODE_MAJOR": [["V", "I"], ["IV", "V", "I"], ["ii", "V", "I"], ["vi", "IV", "V", "I"]],
    "MODE_NATURAL_MINOR": [["V", "i"], ["VII", "i"], ["iv", "V", "i"], ["VI", "VII", "i"]],
    "MODE_DORIAN": [["IV", "i"], ["VII", "i"], ["v", "IV", "i"]],
    "MODE_PHRYGIAN": [["bII", "i"], ["VII", "i"], ["VI", "bII", "i"]],
    "MODE_HARMONIC_MINOR": [["V", "i"], ["ii°", "V", "i"], ["iv", "V", "i"]],
}

SUBSTITUTIONS = {
    "MODE_MAJOR": {"I": ["I", "vi"], "IV": ["IV", "ii"], "V": ["V"], "vi": ["vi", "I"]},
    "MODE_NATURAL_MINOR": {"i": ["i"], "VI": ["VI", "III"], "VII": ["VII", "v"], "iv": ["iv"], "V": ["V", "VII"]},
    "MODE_DORIAN": {"i": ["i"], "IV": ["IV", "ii"], "VII": ["VII", "v"], "v": ["v", "III"]},
    "MODE_PHRYGIAN": {"i": ["i"], "bII": ["bII"], "VII": ["VII"], "VI": ["VI"]},
    "MODE_HARMONIC_MINOR": {"i": ["i"], "iv": ["iv"], "V": ["V"], "VI": ["VI"], "ii°": ["ii°"]},
}


def rotate(tokens: list[str], shift: int) -> list[str]:
    shift %= len(tokens)
    return tokens[shift:] + tokens[:shift]


def stretch(tokens: list[str], target_length: int) -> list[str]:
    result: list[str] = []
    while len(result) < target_length:
        result.extend(tokens)
    return result[:target_length]


def reharmonize(tokens: list[str], mode: str, rng: random.Random) -> list[str]:
    substitutions = SUBSTITUTIONS.get(mode, {})
    result: list[str] = []
    for token in tokens:
        choices = substitutions.get(token)
        result.append(rng.choice(choices) if choices and rng.random() < 0.18 else token)
    return result


def add_cadence(tokens: list[str], mode: str, rng: random.Random) -> list[str]:
    cadences = CADENCES.get(mode, [])
    if not cadences or rng.random() > 0.42:
        return tokens

    cadence = rng.choice(cadences)
    keep = max(1, len(tokens) - len(cadence))
    return [*tokens[:keep], *cadence]


def mutate(tokens: list[str], mode: str, mood: str, rng: random.Random) -> list[str]:
    result = list(tokens)
    if mood == "MOOD_TENSE" and "V" in result and rng.random() < 0.35:
        result.insert(max(1, len(result) - 1), "bII")
    if mood == "MOOD_EPIC" and "VI" in result and rng.random() < 0.30:
        result.append("VII")
    if mood == "MOOD_CALM" and len(result) > 4 and rng.random() < 0.45:
        result = result[:4]
    if mood == "MOOD_DARK" and mode in {"MODE_NATURAL_MINOR", "MODE_PHRYGIAN", "MODE_HARMONIC_MINOR"} and rng.random() < 0.28:
        result = ["i", *result[1:]]
    if mood == "MOOD_BRIGHT" and mode == "MODE_MAJOR" and rng.random() < 0.30:
        result = ["I", *result[1:]]
    return result


def choose_context(rng: random.Random, balanced: bool, index: int) -> tuple[str, str, str]:
    if balanced:
        style, mode = STYLE_MODE_PAIRS[index % len(STYLE_MODE_PAIRS)]
        mood = MOODS[(index // len(STYLE_MODE_PAIRS)) % len(MOODS)]
        return style, mode, mood

    style = rng.choice(list(STYLE_WEIGHTS.keys()))
    mode = rng.choice(list(STYLE_WEIGHTS[style].keys()))
    mood = rng.choice(MOODS)
    return style, mode, mood


def build_examples(count: int, seed: int, balanced: bool) -> list[dict[str, object]]:
    rng = random.Random(seed)
    examples: list[dict[str, object]] = []

    while len(examples) < count:
        style, mode, mood = choose_context(rng, balanced, len(examples))
        base = rng.choice(STYLE_WEIGHTS[style][mode])
        length = rng.choice([4, 4, 6, 8, 8, 12])
        tokens = stretch(rotate(base, rng.randrange(len(base))), length)
        tokens = reharmonize(tokens, mode, rng)
        tokens = add_cadence(tokens, mode, rng)
        tokens = mutate(tokens, mode, mood, rng)
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
    parser.add_argument("--random-context", action="store_true", help="Use random style/mode/mood sampling instead of balanced coverage.")
    args = parser.parse_args()

    examples = build_examples(args.count, args.seed, balanced=not args.random_context)
    output = Path(args.output)
    output.write_text(
        "\n".join(json.dumps(example, ensure_ascii=False, separators=(",", ":")) for example in examples) + "\n",
        encoding="utf-8",
    )
    print(f"written={output} examples={len(examples)} balanced={not args.random_context}")


if __name__ == "__main__":
    main()
