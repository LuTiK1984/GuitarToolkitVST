# MelodyPhraseTransformer

This is the first local training skeleton for the future GuitarToolkit melody/riff model.

The model does not generate audio. It generates symbolic phrase tokens, for example:

```text
D:b3:1/8
R:1/8
```

Meaning:

- `D:b3:1/8` - play scale degree `b3` for one eighth note;
- `R:1/8` - rest for one eighth note.

GuitarToolkit will later map these tokens to real notes, fretboard positions, playback, and MIDI/export tools.

## Dataset format

JSONL: one phrase per line.

```json
{"style":"STYLE_METAL","mode":"MODE_NATURAL_MINOR","mood":"MOOD_DARK","meter":"METER_4_4","bars":2,"progression":["i","VI"],"tokens":["<BOS>","D:1:1/8","D:b3:1/8","<EOS>"]}
```

Important fields:

- `style` - broad musical style.
- `mode` - scale/mode used for degree mapping.
- `mood` - emotional target.
- `meter` - phrase meter: `METER_4_4`, `METER_3_4`, or `METER_6_8`.
- `bars` - phrase length: `1`, `2`, or `4`.
- `progression` - optional chord context.
- `tokens` - phrase tokens with `<BOS>` and `<EOS>`.

## First smoke run

```powershell
cd tools\ml\melody_phrase_transformer
python generate_synthetic_dataset.py --output synthetic_melody_dataset.jsonl --count 5000
python validate_dataset.py --dataset synthetic_melody_dataset.jsonl
python train.py --dataset synthetic_melody_dataset.jsonl --epochs 10 --batch-size 64
python inspect_checkpoint.py --checkpoint runs\melody_phrase_transformer\best_model.pt
python export_onnx.py --checkpoint runs\melody_phrase_transformer\best_model.pt
```

If training is slow, try:

```powershell
python train.py --dataset synthetic_melody_dataset.jsonl --epochs 10 --batch-size 128 --learning-rate 0.0003
```

If CUDA memory is tight, lower batch size:

```powershell
python train.py --dataset synthetic_melody_dataset.jsonl --epochs 10 --batch-size 32
```

## How to read progress

- `train_loss` should go down during training.
- `val_loss` should go down or stabilize.
- `acc` means exact next-token hit rate.
- `top3` means the correct token was among the three most likely model answers.

For melodies, exact `acc` is naturally harder than for progressions because there are more plausible answers. A useful early model should show improving `val_loss`, decent `top3`, and inspection output that stays inside musical phrase tokens instead of random symbols.

## What counts as a first win

This first version is a pipeline win, not a final music-quality win:

- dataset validates;
- training finishes;
- checkpoint inspection returns phrase tokens;
- ONNX export succeeds;
- generated durations can be checked against meter/bar length.

After that, the real work is improving the dataset rules and later connecting the ONNX model to the Melody tab.
