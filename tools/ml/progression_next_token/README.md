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
.\.venv\Scripts\python generate_synthetic_dataset.py --output synthetic_dataset.jsonl --count 5000
.\.venv\Scripts\python validate_dataset.py --dataset synthetic_dataset.jsonl
.\.venv\Scripts\python train.py --dataset synthetic_dataset.jsonl --epochs 40
.\.venv\Scripts\python inspect_checkpoint.py --previous "<BOS>,i,VI"
.\.venv\Scripts\python export_onnx.py
```

`sample_dataset.jsonl` нужен только для проверки пайплайна. Для настоящей модели понадобится отдельный датасет.

## Выход

Обучение пишет:

```text
runs/progression_next_token/ProgressionNextTokenModel.pt
runs/progression_next_token/best_model.pt
runs/progression_next_token/training_config.json
runs/progression_next_token/metrics.json
```

`ProgressionNextTokenModel.pt` - последний checkpoint. `best_model.pt` - лучший checkpoint по validation loss. `inspect_checkpoint.py` и `export_onnx.py` по умолчанию берут именно `best_model.pt`.

## Накопительное обучение

Чтобы продолжить обучение уже существующей модели, используй `--resume`:

```powershell
.\.venv\Scripts\python train.py --dataset synthetic_dataset.jsonl --epochs 20 --resume runs/progression_next_token/ProgressionNextTokenModel.pt
```

Это не начинает с нуля. Скрипт загружает веса модели, состояние optimizer, номер прошлой эпохи и лучший validation loss, а затем добавляет новые эпохи поверх старых.

Если нужно продолжить с весов, но пересоздать optimizer, добавь:

```powershell
.\.venv\Scripts\python train.py --dataset synthetic_dataset.jsonl --epochs 20 --resume runs/progression_next_token/ProgressionNextTokenModel.pt --reset-optimizer
```

Для регулярных снапшотов:

```powershell
.\.venv\Scripts\python train.py --dataset synthetic_dataset.jsonl --epochs 40 --save-every 10
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

## Как фиксировать победу

Первую победу считаем не по красоте интерфейса, а по повторяемому ML-контуру:

- training loss падает;
- validation loss падает или стабилизируется без резкого разлета;
- `top3_accuracy` заметно выше случайного выбора;
- `inspect_checkpoint.py` на seed вроде `<BOS>,i,VI` показывает музыкально ожидаемые варианты: `VII`, `III`, `iv`, `V`;
- в top-k не появляются служебные токены вроде `STYLE_*`, `MOOD_*`, `<PAD>` или `<UNK>`;
- ONNX экспорт проходит без ошибки;
- после подключения ONNX Runtime программа получает не demo fallback, а ответ реальной модели.

На синтетике хороший результат не доказывает “умную музыку”, но доказывает, что пайплайн живой. После этого главным улучшением становится качество датасета.
