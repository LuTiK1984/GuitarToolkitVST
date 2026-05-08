using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GuitarToolkit.MLTrainer;

public sealed class TrainerForm : Form
{
    private static readonly Regex EpochRegex = new(
        @"epoch=(?<epoch>\d+)/(?<total>\d+)\s+train_loss=(?<train>[0-9.]+)\s+val_loss=(?<val>[0-9.]+)\s+acc=(?<acc>[0-9.]+)\s+top3=(?<top3>[0-9.]+)",
        RegexOptions.Compiled);
    private static readonly Regex ProgressRegex = new(
        @"train_progress\s+epoch=(?<epoch>\d+)/(?<total>\d+)\s+batch=(?<batch>\d+)/(?<batches>\d+)\s+percent=(?<percent>[0-9.]+)\s+train_loss=(?<loss>[0-9.]+)",
        RegexOptions.Compiled);

    private readonly ProcessRunner _runner = new();
    private readonly CancellationTokenSource _shutdown = new();

    private readonly TextBox _pythonBox = new() { Text = "python" };
    private readonly TextBox _progressionRootBox = new();
    private readonly TextBox _logBox = new();
    private readonly ListView _epochList = new();
    private readonly TextBox _previewBox = new();
    private readonly TextBox _resultBox = new();
    private readonly ListView _metricsList = new();

    private readonly TextBox _datasetBox = new() { Text = "synthetic_dataset_gui.jsonl" };
    private readonly NumericUpDown _datasetCountBox = new() { Minimum = 100, Maximum = 1_000_000, Value = 80000, Increment = 1000 };
    private readonly NumericUpDown _seedBox = new() { Minimum = 1, Maximum = 999999, Value = 7777 };
    private readonly ComboBox _profileBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };

    private readonly TextBox _outputDirBox = new() { Text = @"runs\progression_gui" };
    private readonly TextBox _resumeBox = new() { Text = @"runs\progression_diverse_plateau\best_model.pt" };
    private readonly NumericUpDown _epochsBox = new() { Minimum = 1, Maximum = 500, Value = 40 };
    private readonly NumericUpDown _batchBox = new() { Minimum = 1, Maximum = 4096, Value = 256 };
    private readonly NumericUpDown _learningRateBox = new() { DecimalPlaces = 5, Minimum = 0.00001M, Maximum = 1, Increment = 0.00005M, Value = 0.00010M };
    private readonly NumericUpDown _labelSmoothingBox = new() { DecimalPlaces = 3, Minimum = 0, Maximum = 0.5M, Increment = 0.005M, Value = 0.040M };
    private readonly NumericUpDown _progressEveryBox = new() { Minimum = 0, Maximum = 10000, Value = 100, Increment = 10 };
    private readonly CheckBox _resetOptimizerBox = new() { Text = "Начать с новым optimizer при дообучении", Checked = true, AutoSize = true };
    private readonly CheckBox _cpuBox = new() { Text = "Отключить GPU и обучать на CPU", AutoSize = true };
    private readonly ProgressBar _trainProgress = new() { Minimum = 0, Maximum = 1000, Dock = DockStyle.Top, Height = 18 };
    private readonly Label _progressLabel = new() { Text = "Прогресс эпохи: ожидание запуска", AutoSize = true };
    private readonly ToolTip _toolTip = new();

    private readonly TextBox _checkpointBox = new() { Text = @"runs\progression_gui\best_model.pt" };
    private readonly TextBox _previousBox = new() { Text = "<BOS>,i,VI" };
    private readonly ComboBox _styleBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _modeBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _moodBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };

    public TrainerForm()
    {
        Text = "GuitarToolkit ML Trainer";
        MinimumSize = new Size(1180, 760);
        StartPosition = FormStartPosition.CenterScreen;

        _progressionRootBox.Text = FindProgressionRoot();
        _profileBox.Items.AddRange(["focused", "balanced", "diverse"]);
        _profileBox.SelectedItem = "diverse";
        _styleBox.Items.AddRange(["STYLE_METAL", "STYLE_ROCK", "STYLE_POP", "STYLE_AMBIENT", "STYLE_BLUES"]);
        _styleBox.SelectedItem = "STYLE_METAL";
        _modeBox.Items.AddRange(["MODE_NATURAL_MINOR", "MODE_MAJOR", "MODE_DORIAN", "MODE_PHRYGIAN", "MODE_HARMONIC_MINOR"]);
        _modeBox.SelectedItem = "MODE_NATURAL_MINOR";
        _moodBox.Items.AddRange(["MOOD_DARK", "MOOD_EPIC", "MOOD_BRIGHT", "MOOD_CALM", "MOOD_TENSE"]);
        _moodBox.SelectedItem = "MOOD_DARK";
        ConfigureToolTips();

        Controls.Add(BuildLayout());
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _shutdown.Cancel();
        _runner.Stop();
        base.OnFormClosing(e);
    }

    private Control BuildLayout()
    {
        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(BuildProgressionTab());
        tabs.TabPages.Add(BuildMelodyTransformerTab());
        tabs.TabPages.Add(BuildSettingsTab());

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(10)
        };
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 62));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 38));
        root.Controls.Add(tabs, 0, 0);
        root.Controls.Add(BuildOutputPanel(), 0, 1);
        return root;
    }

    private TabPage BuildProgressionTab()
    {
        var page = new TabPage("Progression GRU/LSTM");
        var columns = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Padding = new Padding(8)
        };
        columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));

        columns.Controls.Add(BuildDatasetPanel(), 0, 0);
        columns.Controls.Add(BuildTrainingPanel(), 1, 0);
        columns.Controls.Add(BuildEvaluationPanel(), 2, 0);
        page.Controls.Add(columns);
        return page;
    }

    private Control BuildDatasetPanel()
    {
        var panel = Panel("Датасет");
        AddRow(panel, "Файл", _datasetBox);
        AddRow(panel, "Количество", _datasetCountBox);
        AddRow(panel, "Seed", _seedBox);
        AddRow(panel, "Профиль", _profileBox);
        AddButtonRow(panel,
            Button("Сгенерировать", GenerateDataset_Click),
            Button("Проверить", ValidateDataset_Click),
            Button("Превью", PreviewDataset_Click));

        var note = Note("focused = точность, balanced = базовый баланс, diverse = больше неожиданных, но музыкальных вариантов.");
        panel.Controls.Add(note);
        return panel;
    }

    private Control BuildTrainingPanel()
    {
        var panel = Panel("Обучение");
        AddRow(panel, "Output dir", _outputDirBox);
        AddRow(panel, "Resume", _resumeBox);
        AddRow(panel, "Эпохи", _epochsBox);
        AddRow(panel, "Batch", _batchBox);
        AddRow(panel, "Скорость обучения (learning rate)", _learningRateBox);
        AddRow(panel, "Мягкость ответов (label smoothing)", _labelSmoothingBox);
        AddRow(panel, "Показывать прогресс каждые N batches", _progressEveryBox);
        panel.Controls.Add(_resetOptimizerBox);
        panel.Controls.Add(_cpuBox);
        panel.Controls.Add(_progressLabel);
        panel.Controls.Add(_trainProgress);
        AddButtonRow(panel, Button("Старт", Train_Click), Button("Стоп", Stop_Click), Button("Открыть runs", OpenRuns_Click));

        var note = Note("Для RTX 3060 Ti обычно стартуй с batch 256. Если будет CUDA out of memory, снизь до 128.");
        panel.Controls.Add(note);
        return panel;
    }

    private Control BuildEvaluationPanel()
    {
        var panel = Panel("Проверка и экспорт");
        AddRow(panel, "Checkpoint", _checkpointBox);
        AddRow(panel, "Previous", _previousBox);
        AddRow(panel, "Style", _styleBox);
        AddRow(panel, "Mode", _modeBox);
        AddRow(panel, "Mood", _moodBox);
        AddButtonRow(panel,
            Button("Inspect", Inspect_Click),
            Button("Evaluate", Evaluate_Click),
            Button("Export ONNX", Export_Click));
        AddButtonRow(panel, Button("Install in app", Install_Click), Button("Папка модели", OpenModelFolder_Click));

        var note = Note("Inspect показывает вероятности следующей ступени. Evaluate считает энтропию, top3 и разрезы по style/mode/mood.");
        panel.Controls.Add(note);
        return panel;
    }

    private TabPage BuildMelodyTransformerTab()
    {
        var page = new TabPage("Melody Transformer");
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(14)
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        var left = Panel("Будущая модель");
        left.Controls.Add(Note("Эта вкладка заранее резервирует место под вторую модель: маленький Transformer для коротких мелодий и риффов в выбранном размере такта."));
        left.Controls.Add(Note("План входа: стиль, лад, настроение, размер, длина, опорная прогрессия. План выхода: токены нот/пауз/длительностей, которые основная программа потом сыграет своим синтезом."));

        var right = Panel("Будущие операции");
        right.Controls.Add(Note("Когда появятся scripts для melody_transformer, сюда добавим генерацию датасета, обучение, evaluate, export ONNX и тестовый preview MIDI/нот."));
        right.Controls.Add(Note("Сейчас вкладка намеренно не запускает несуществующие команды, чтобы не смешивать рабочую progression-модель и будущий Transformer."));

        panel.Controls.Add(left, 0, 0);
        panel.Controls.Add(right, 1, 0);
        page.Controls.Add(panel);
        return page;
    }

    private TabPage BuildSettingsTab()
    {
        var page = new TabPage("Настройки");
        var panel = Panel("Пути");
        AddRow(panel, "Python", _pythonBox);
        AddRow(panel, "Progression tools", _progressionRootBox);
        AddButtonRow(panel, Button("Проверить GPU", CheckGpu_Click), Button("Открыть tools", OpenTools_Click));
        panel.Controls.Add(Note("Если CUDA подключена правильно, проверка GPU покажет torch.cuda.is_available() = True и имя видеокарты."));
        page.Controls.Add(panel);
        return page;
    }

    private Control BuildOutputPanel()
    {
        var tabs = new TabControl { Dock = DockStyle.Fill };

        _logBox.Dock = DockStyle.Fill;
        _logBox.Multiline = true;
        _logBox.ScrollBars = ScrollBars.Both;
        _logBox.Font = new Font("Consolas", 9);
        tabs.TabPages.Add(new TabPage("Лог") { Controls = { _logBox } });

        _epochList.Dock = DockStyle.Fill;
        _epochList.View = View.Details;
        _epochList.FullRowSelect = true;
        _epochList.Columns.Add("Epoch", 80);
        _epochList.Columns.Add("Train loss", 100);
        _epochList.Columns.Add("Val loss", 100);
        _epochList.Columns.Add("Acc", 80);
        _epochList.Columns.Add("Top3", 80);
        tabs.TabPages.Add(new TabPage("Эпохи") { Controls = { _epochList } });

        _previewBox.Dock = DockStyle.Fill;
        _previewBox.Multiline = true;
        _previewBox.ScrollBars = ScrollBars.Both;
        _previewBox.Font = new Font("Consolas", 9);
        tabs.TabPages.Add(new TabPage("Превью датасета") { Controls = { _previewBox } });

        _resultBox.Dock = DockStyle.Fill;
        _resultBox.Multiline = true;
        _resultBox.ScrollBars = ScrollBars.Both;
        _resultBox.Font = new Font("Consolas", 9);
        tabs.TabPages.Add(new TabPage("Результат") { Controls = { _resultBox } });

        _metricsList.Dock = DockStyle.Fill;
        _metricsList.View = View.Details;
        _metricsList.FullRowSelect = true;
        _metricsList.Columns.Add("Метрика", 240);
        _metricsList.Columns.Add("Значение", 120);
        _metricsList.Columns.Add("Смысл", 520);
        tabs.TabPages.Add(new TabPage("Оценка модели") { Controls = { _metricsList } });

        return tabs;
    }

    private async void GenerateDataset_Click(object? sender, EventArgs e)
    {
        await RunPythonAsync("generate_synthetic_dataset.py", $"--output {Quote(_datasetBox.Text)} --count {_datasetCountBox.Value:0} --seed {_seedBox.Value:0} --profile {_profileBox.Text}");
    }

    private async void ValidateDataset_Click(object? sender, EventArgs e)
    {
        await RunPythonAsync("validate_dataset.py", $"--dataset {Quote(_datasetBox.Text)}");
    }

    private void PreviewDataset_Click(object? sender, EventArgs e)
    {
        string path = ResolveToolPath(_datasetBox.Text);
        if (!File.Exists(path))
        {
            AppendLog($"dataset not found: {path}");
            return;
        }

        _previewBox.Text = string.Join(Environment.NewLine, File.ReadLines(path).Take(120));
    }

    private async void Train_Click(object? sender, EventArgs e)
    {
        _epochList.Items.Clear();
        _trainProgress.Value = 0;
        _progressLabel.Text = "Прогресс эпохи: запуск обучения";
        string args =
            $"--dataset {Quote(_datasetBox.Text)} " +
            $"--epochs {_epochsBox.Value:0} " +
            $"--batch-size {_batchBox.Value:0} " +
            $"--learning-rate {DecimalText(_learningRateBox.Value)} " +
            $"--label-smoothing {DecimalText(_labelSmoothingBox.Value)} " +
            $"--output-dir {Quote(_outputDirBox.Text)} " +
            $"--resume {Quote(_resumeBox.Text)} " +
            $"--save-every 10 " +
            $"--progress-every {_progressEveryBox.Value:0}";

        if (_resetOptimizerBox.Checked)
            args += " --reset-optimizer";
        if (_cpuBox.Checked)
            args += " --cpu";

        await RunPythonAsync("train.py", args);
    }

    private void Stop_Click(object? sender, EventArgs e)
    {
        _runner.Stop();
        AppendLog("stop requested");
    }

    private async void Inspect_Click(object? sender, EventArgs e)
    {
        string args =
            $"--checkpoint {Quote(_checkpointBox.Text)} " +
            $"--previous {Quote(_previousBox.Text)} " +
            $"--style {_styleBox.Text} --mode {_modeBox.Text} --mood {_moodBox.Text}";
        await RunPythonAsync("inspect_checkpoint.py", args, captureResult: true);
    }

    private async void Evaluate_Click(object? sender, EventArgs e)
    {
        string args = $"--checkpoint {Quote(_checkpointBox.Text)} --top-k 8";
        await RunPythonAsync("evaluate_checkpoint.py", args, captureResult: true, parseEvaluation: true);
    }

    private async void Export_Click(object? sender, EventArgs e)
    {
        string output = Path.Combine(Path.GetDirectoryName(_checkpointBox.Text) ?? string.Empty, "ProgressionNextTokenModel.onnx");
        string args = $"--checkpoint {Quote(_checkpointBox.Text)} --output {Quote(output)}";
        await RunPythonAsync("export_onnx.py", args);
    }

    private async void Install_Click(object? sender, EventArgs e)
    {
        string model = Path.Combine(Path.GetDirectoryName(_checkpointBox.Text) ?? string.Empty, "ProgressionNextTokenModel.onnx");
        string source = ResolveToolPath(model);
        if (!File.Exists(source))
        {
            AppendLog($"model not found: {source}");
            return;
        }

        string targetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GuitarToolkit", "models");
        Directory.CreateDirectory(targetDir);
        string targetPath = Path.Combine(targetDir, "ProgressionNextTokenModel.onnx");
        File.Copy(source, targetPath, overwrite: true);
        AppendLog($"installed={targetPath}");
    }

    private async void CheckGpu_Click(object? sender, EventArgs e)
    {
        await RunProcessAsync(
            _pythonBox.Text,
            "-c \"import torch; print(torch.__version__, torch.version.cuda, torch.cuda.is_available(), torch.cuda.get_device_name(0) if torch.cuda.is_available() else 'CPU')\"",
            _progressionRootBox.Text);
    }

    private void OpenRuns_Click(object? sender, EventArgs e) => OpenFolder(ResolveToolPath("runs"));

    private void OpenTools_Click(object? sender, EventArgs e) => OpenFolder(_progressionRootBox.Text);

    private void OpenModelFolder_Click(object? sender, EventArgs e)
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GuitarToolkit", "models");
        OpenFolder(path);
    }

    private async Task RunPythonAsync(string script, string arguments, bool captureResult = false, bool parseEvaluation = false)
    {
        await RunProcessAsync(_pythonBox.Text, $"{script} {arguments}", _progressionRootBox.Text, captureResult, parseEvaluation);
    }

    private async Task RunProcessAsync(string fileName, string arguments, string workingDirectory, bool captureResult = false, bool parseEvaluation = false)
    {
        if (_runner.IsRunning)
        {
            AppendLog("another process is already running");
            return;
        }

        var result = new StringBuilder();
        AppendLog($"> {fileName} {arguments}");
        try
        {
            int exitCode = await _runner.RunAsync(
                fileName,
                arguments,
                workingDirectory,
                line =>
                {
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        AppendLog(line);
                        ParseEpoch(line);
                        ParseProgress(line);
                        if (captureResult)
                            result.AppendLine(line);
                    }));
                },
                _shutdown.Token);

            AppendLog($"exit={exitCode}");
            if (captureResult)
            {
                _resultBox.Text = result.ToString();
                if (parseEvaluation)
                    RenderEvaluationMetrics(result.ToString());
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            AppendLog(ex.Message);
        }
    }

    private void ParseEpoch(string line)
    {
        Match match = EpochRegex.Match(line);
        if (!match.Success)
            return;

        var item = new ListViewItem($"{match.Groups["epoch"].Value}/{match.Groups["total"].Value}");
        item.SubItems.Add(match.Groups["train"].Value);
        item.SubItems.Add(match.Groups["val"].Value);
        item.SubItems.Add(match.Groups["acc"].Value);
        item.SubItems.Add(match.Groups["top3"].Value);
        _epochList.Items.Add(item);
        item.EnsureVisible();
        _trainProgress.Value = 1000;
        _progressLabel.Text = $"Эпоха {match.Groups["epoch"].Value}/{match.Groups["total"].Value}: validation готова";
    }

    private void ParseProgress(string line)
    {
        Match match = ProgressRegex.Match(line);
        if (!match.Success)
            return;

        double percent = double.Parse(match.Groups["percent"].Value, CultureInfo.InvariantCulture);
        _trainProgress.Value = Math.Clamp((int)Math.Round(percent * 10), 0, 1000);
        _progressLabel.Text =
            $"Эпоха {match.Groups["epoch"].Value}/{match.Groups["total"].Value}: " +
            $"{percent:0.0}% " +
            $"batch {match.Groups["batch"].Value}/{match.Groups["batches"].Value}, " +
            $"loss {match.Groups["loss"].Value}";
    }

    private void AppendLog(string text)
    {
        _logBox.AppendText(text + Environment.NewLine);
    }

    private void RenderEvaluationMetrics(string json)
    {
        _metricsList.Items.Clear();

        try
        {
            using JsonDocument document = JsonDocument.Parse(json);
            JsonElement summary = document.RootElement.GetProperty("summary");

            AddMetric(summary, "overall_score_percent", "Итоговая оценка", "%", "Сводный балл: разнообразие, музыкальность, настроение, стиль и баланс уверенности.");
            AddMetric(summary, "diversity_score_percent", "Разнообразие", "%", "Насколько модель оставляет живой выбор вместо одного жесткого ответа.");
            AddMetric(summary, "musicality_score_percent", "Музыкальное попадание", "%", "Масса вероятности и top-1 внутри допустимых для лада ступеней.");
            AddMetric(summary, "mood_fit_score_percent", "Попадание в настроение", "%", "Насколько ответы соответствуют выбранному mood.");
            AddMetric(summary, "style_fit_score_percent", "Попадание в стиль", "%", "Насколько ответы соответствуют выбранному style.");
            AddMetric(summary, "confidence_balance_percent", "Баланс уверенности", "%", "Штрафует слишком зажатую и слишком размазанную модель.");
            AddMetric(summary, "distinct_top1_percent", "Уникальность top-1", "%", "Сколько разных первых ответов модель дала на тестовый набор.");
            AddMetric(summary, "avg_entropy", "Средняя энтропия", "", "Сырая мера вариативности распределения.");
            AddMetric(summary, "avg_top3_mass", "Масса top-3", "", "Сколько вероятности забирают три первых варианта.");
            AddMetric(summary, "top1_musical_hit_percent", "Top-1 в ладу", "%", "Процент тестов, где первый ответ попал в допустимые ступени.");
            AddMetric(summary, "top1_mood_hit_percent", "Top-1 в настроении", "%", "Процент тестов, где первый ответ попал в mood-набор.");
            AddMetric(summary, "top1_style_hit_percent", "Top-1 в стиле", "%", "Процент тестов, где первый ответ попал в style-набор.");
        }
        catch (JsonException ex)
        {
            AppendLog($"evaluation metrics parse failed: {ex.Message}");
        }
        catch (KeyNotFoundException ex)
        {
            AppendLog($"evaluation summary is missing expected field: {ex.Message}");
        }
    }

    private void AddMetric(JsonElement summary, string propertyName, string label, string suffix, string description)
    {
        if (!summary.TryGetProperty(propertyName, out JsonElement value))
            return;

        string text = value.ValueKind switch
        {
            JsonValueKind.Number => value.TryGetInt32(out int integer)
                ? integer.ToString(CultureInfo.InvariantCulture)
                : value.GetDouble().ToString("0.####", CultureInfo.InvariantCulture),
            _ => value.ToString()
        };

        var item = new ListViewItem(label);
        item.SubItems.Add(string.IsNullOrEmpty(suffix) ? text : $"{text}{suffix}");
        item.SubItems.Add(description);
        _metricsList.Items.Add(item);
    }

    private string ResolveToolPath(string path)
    {
        if (Path.IsPathRooted(path))
            return path;

        return Path.GetFullPath(Path.Combine(_progressionRootBox.Text, path));
    }

    private static string FindProgressionRoot()
    {
        string current = AppContext.BaseDirectory;
        for (int i = 0; i < 8; i++)
        {
            string candidate = Path.GetFullPath(Path.Combine(current, "..", "..", "..", "..", "progression_next_token"));
            if (Directory.Exists(candidate))
                return candidate;

            current = Path.GetFullPath(Path.Combine(current, ".."));
        }

        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "progression_next_token"));
    }

    private static void OpenFolder(string path)
    {
        Directory.CreateDirectory(path);
        Process.Start(new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        });
    }

    private static string Quote(string value)
    {
        return "\"" + value.Replace("\"", "\\\"", StringComparison.Ordinal) + "\"";
    }

    private static string DecimalText(decimal value)
    {
        return value.ToString("0.#####", CultureInfo.InvariantCulture);
    }

    private static TableLayoutPanel Panel(string title)
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            ColumnCount = 1,
            RowCount = 0,
            Padding = new Padding(10)
        };
        panel.Controls.Add(new Label
        {
            Text = title,
            AutoSize = true,
            Font = new Font(SystemFonts.MessageBoxFont ?? Control.DefaultFont, FontStyle.Bold),
            Margin = new Padding(0, 0, 0, 10)
        });
        return panel;
    }

    private static void AddRow(TableLayoutPanel panel, string label, Control control)
    {
        control.Dock = DockStyle.Top;
        control.Margin = new Padding(0, 0, 0, 10);
        panel.Controls.Add(new Label { Text = label, AutoSize = true, Margin = new Padding(0, 0, 0, 3) });
        panel.Controls.Add(control);
    }

    private static void AddButtonRow(TableLayoutPanel panel, params Button[] buttons)
    {
        var row = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 5, 0, 10) };
        foreach (Button button in buttons)
            row.Controls.Add(button);

        panel.Controls.Add(row);
    }

    private static Button Button(string text, EventHandler handler)
    {
        var button = new Button { Text = text, AutoSize = true, Height = 30, Margin = new Padding(0, 0, 8, 0) };
        button.Click += handler;
        return button;
    }

    private static Label Note(string text)
    {
        return new Label
        {
            Text = text,
            AutoSize = false,
            Height = 72,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 8, 0, 0)
        };
    }

    private void ConfigureToolTips()
    {
        _toolTip.SetToolTip(_learningRateBox, "Размер шага обучения. Меньше = медленнее, но аккуратнее; для финального fine-tune обычно 0.00005-0.0001.");
        _toolTip.SetToolTip(_labelSmoothingBox, "Оставляет часть вероятности альтернативным аккордам. Больше = вариативнее, но выше риск странных ходов.");
        _toolTip.SetToolTip(_progressEveryBox, "Как часто train.py пишет прогресс внутри эпохи. 0 отключает промежуточный вывод.");
        _toolTip.SetToolTip(_resetOptimizerBox, "Веса модели сохраняются, но AdamW начинает без старой инерции. Обычно включать при новом датасете или learning rate.");
        _toolTip.SetToolTip(_cpuBox, "Полезно только для отладки. Для нормального обучения оставь выключенным, чтобы работала видеокарта.");
    }
}
