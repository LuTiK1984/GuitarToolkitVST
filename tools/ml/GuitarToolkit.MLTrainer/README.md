# GuitarToolkit ML Trainer

Small Windows Forms utility for local GuitarToolkit model training.

## Purpose

This tool wraps the Python scripts in `tools/ml/progression_next_token` so training can be managed from one window:

- generate and validate synthetic datasets;
- preview dataset rows;
- start/stop training runs;
- watch epoch metrics as they arrive;
- inspect a checkpoint on a single musical prompt;
- evaluate checkpoint diversity by style, mode, and mood;
- compare multiple checkpoints in one table;
- keep a local JSONL history of model evaluation cards;
- export ONNX;
- install the exported model into `%APPDATA%\GuitarToolkit\models`.

The second tab reserves the workflow for the future melody/riff Transformer model. It is intentionally non-executing until the transformer dataset and training scripts exist.

## Run

From the repository root:

```powershell
dotnet run --project tools\ml\GuitarToolkit.MLTrainer\GuitarToolkit.MLTrainer.csproj --configuration Debug
```

The tool expects Python dependencies from `tools/ml/progression_next_token/requirements.txt`.
