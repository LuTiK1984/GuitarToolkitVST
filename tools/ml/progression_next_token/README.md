# ProgressionNextTokenModel

Русский рабочий черновик для локальной модели Inspiration Engine.

Модель решает первую узкую задачу: по стилю, ладу, настроению и предыдущим римским ступеням предсказать следующую ступень.

## Формат датасета

Файл JSONL: одна прогрессия на строку.

```json
{"style":"STYLE_METAL","mode":"MODE_NATURAL_MINOR","mood":"MOOD_DARK","tokens":["<BOS>","i","VI","VII","i","<EOS>"]}
```

Датасет должен быть лицензионно чистым. Табулатуры, MIDI и чужие песни нельзя складывать сюда без понятных прав.

## Первый запуск

```powershell
cd tools/ml/progression_next_token
python -m venv .venv
.\.venv\Scripts\pip install -r requirements.txt
.\.venv\Scripts\python train.py --dataset sample_dataset.jsonl --epochs 5
.\.venv\Scripts\python export_onnx.py
```

`sample_dataset.jsonl` нужен только для проверки пайплайна. Для настоящей модели понадобится отдельный датасет.

## Выход

Обучение пишет:

```text
runs/progression_next_token/ProgressionNextTokenModel.pt
runs/progression_next_token/training_config.json
```

Экспорт пишет:

```text
runs/progression_next_token/ProgressionNextTokenModel.onnx
```

Для ручной проверки в программе будущий ONNX-файл нужно будет положить сюда:

```text
%AppData%\GuitarToolkit\models\ProgressionNextTokenModel.onnx
```

## Почему так

Модель не генерирует звук. Она генерирует вероятности следующего музыкального токена. GuitarToolkit сам выбирает токен через temperature/top-k, проверяет результат и проигрывает аккорды встроенными инструментами.
