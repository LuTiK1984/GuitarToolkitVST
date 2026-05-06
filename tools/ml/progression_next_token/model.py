from __future__ import annotations

import json
from dataclasses import dataclass
from pathlib import Path

import torch
from torch import nn


@dataclass(frozen=True)
class Vocabulary:
    token_to_id: dict[str, int]
    id_to_token: list[str]
    style_tokens: list[str]
    mode_tokens: list[str]
    mood_tokens: list[str]
    progression_tokens: list[str]

    @property
    def pad_id(self) -> int:
        return self.token_to_id["<PAD>"]

    @property
    def unk_id(self) -> int:
        return self.token_to_id["<UNK>"]

    @classmethod
    def load(cls, path: str | Path) -> "Vocabulary":
        data = json.loads(Path(path).read_text(encoding="utf-8"))
        tokens = (
            data["special_tokens"]
            + data["style_tokens"]
            + data["mode_tokens"]
            + data["mood_tokens"]
            + data["progression_tokens"]
        )
        return cls(
            token_to_id={token: index for index, token in enumerate(tokens)},
            id_to_token=tokens,
            style_tokens=data["style_tokens"],
            mode_tokens=data["mode_tokens"],
            mood_tokens=data["mood_tokens"],
            progression_tokens=data["progression_tokens"],
        )

    def encode(self, token: str) -> int:
        return self.token_to_id.get(token, self.unk_id)


class ProgressionNextTokenModel(nn.Module):
    def __init__(
        self,
        vocabulary_size: int,
        embedding_size: int = 96,
        hidden_size: int = 192,
        layers: int = 1,
        dropout: float = 0.0,
        rnn_type: str = "gru",
    ) -> None:
        super().__init__()
        self.embedding = nn.Embedding(vocabulary_size, embedding_size)
        rnn_cls = nn.LSTM if rnn_type.lower() == "lstm" else nn.GRU
        self.rnn = rnn_cls(
            input_size=embedding_size,
            hidden_size=hidden_size,
            num_layers=layers,
            batch_first=True,
            dropout=dropout if layers > 1 else 0.0,
        )
        self.output = nn.Linear(hidden_size, vocabulary_size)

    def forward(
        self,
        style_id: torch.Tensor,
        mode_id: torch.Tensor,
        mood_id: torch.Tensor,
        previous_tokens: torch.Tensor,
    ) -> torch.Tensor:
        prefix = torch.stack([style_id, mode_id, mood_id], dim=1)
        sequence = torch.cat([prefix, previous_tokens], dim=1)
        embedded = self.embedding(sequence)
        rnn_output, _ = self.rnn(embedded)
        last_hidden = rnn_output[:, -1, :]
        return self.output(last_hidden)
