using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using GuitarToolkit.Core.DSP;
using GuitarToolkit.Core.Models;

namespace GuitarToolkit.UI;

public partial class IntervalTrainerView : UserControl
{
    private IAudioPlayback? _audio;
    private readonly IntervalTrainer _trainer = new();
    private float[]? _lastSamples;
    private bool _questionActive = false;
    private DispatcherTimer? _autoAdvanceTimer;

    private static readonly SolidColorBrush BrushCorrect = new(Color.FromRgb(166, 227, 161));
    private static readonly SolidColorBrush BrushWrong = new(Color.FromRgb(243, 139, 168));
    private static readonly SolidColorBrush BrushAccent = new(Color.FromRgb(203, 166, 247));
    private static readonly SolidColorBrush BrushBtn = new(Color.FromRgb(74, 56, 96));
    private static readonly SolidColorBrush BrushText = new(Color.FromRgb(205, 214, 244));
    private static readonly SolidColorBrush BrushDim = new(Color.FromRgb(124, 111, 150));

    static IntervalTrainerView()
    {
        BrushCorrect.Freeze(); BrushWrong.Freeze(); BrushAccent.Freeze();
        BrushBtn.Freeze(); BrushText.Freeze(); BrushDim.Freeze();
    }

    public IntervalTrainerView()
    {
        InitializeComponent();
    }

    public void Initialize(IAudioPlayback audio)
    {
        _audio = audio;
        BuildAnswerButtons();

        _autoAdvanceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1500) };
        _autoAdvanceTimer.Tick += (s, e) =>
        {
            _autoAdvanceTimer.Stop();
            if (_audio != null) PlayNewQuestion();
        };
    }

    private void UpdateVisibleButtons()
    {
        AnswerButtons.Items.Clear();

        int max = _trainer.MaxSemitones;
        int start = _trainer.IncludeUnison ? 0 : 1;

        for (int i = start; i <= max && i < IntervalTrainer.AllIntervals.Length; i++)
        {
            var interval = IntervalTrainer.AllIntervals[i];
            var btn = new Button
            {
                Content = $"{interval.ShortName}\n{interval.Name}",
                Width = 110, Height = 52,
                FontSize = 12, FontWeight = FontWeights.Bold,
                Background = BrushBtn, Foreground = BrushText,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(4),
                Tag = interval.Semitones,
                IsEnabled = false,
                ToolTip = $"{interval.Name} ({interval.Semitones} полутонов)"
            };
            btn.Click += Answer_Click;
            AnswerButtons.Items.Add(btn);
        }
    }

    private void BuildAnswerButtons()
    {
        AnswerButtons.Items.Clear();
        UpdateVisibleButtons();
    }

    private void SetButtonsEnabled(bool enabled)
    {
        foreach (var item in AnswerButtons.Items)
            if (item is Button btn) btn.IsEnabled = enabled;
    }

    private void ResetButtonColors()
    {
        foreach (var item in AnswerButtons.Items)
            if (item is Button btn) btn.Background = BrushBtn;
    }

    // ── Играть ───────────────────────────────────────────────

    private void PlayNewQuestion()
    {
        if (_audio == null) return;

        ResetButtonColors();
        var (f1, f2) = _trainer.GenerateQuestion();
        _lastSamples = NoteSynth.GenerateInterval(f1, f2, _audio.SampleRate);
        _audio.PlaySamples(_lastSamples);

        _questionActive = true;
        SetButtonsEnabled(true);
        RepeatButton.IsEnabled = true;

        ResultLabel.Text = "Слушай и выбери интервал...";
        ResultLabel.Foreground = BrushAccent;
        ResultBorder.Background = new SolidColorBrush(Color.FromArgb(30, 203, 166, 247));
    }

    private void Play_Click(object sender, RoutedEventArgs e) => PlayNewQuestion();

    private void Repeat_Click(object sender, RoutedEventArgs e)
    {
        if (_lastSamples != null && _audio != null)
            _audio.PlaySamples(_lastSamples);
    }

    // ── Ответ ────────────────────────────────────────────────

    private void Answer_Click(object sender, RoutedEventArgs e)
    {
        if (!_questionActive || sender is not Button btn) return;

        int answered = (int)btn.Tag;
        bool correct = _trainer.CheckAnswer(answered);

        _questionActive = false;
        SetButtonsEnabled(false);

        foreach (var item in AnswerButtons.Items)
        {
            if (item is Button b)
            {
                int tag = (int)b.Tag;
                if (tag == _trainer.CurrentInterval.Semitones)
                    b.Background = BrushCorrect;
                else if (tag == answered && !correct)
                    b.Background = BrushWrong;
            }
        }

        if (correct)
        {
            ResultLabel.Text = $"✓ Верно! {_trainer.CurrentInterval.Name}";
            ResultLabel.Foreground = BrushCorrect;
            ResultBorder.Background = new SolidColorBrush(Color.FromArgb(30, 166, 227, 161));
        }
        else
        {
            ResultLabel.Text = $"✗ Нет! Было: {_trainer.CurrentInterval.Name}";
            ResultLabel.Foreground = BrushWrong;
            ResultBorder.Background = new SolidColorBrush(Color.FromArgb(30, 243, 139, 168));
        }

        UpdateStats();

        // Автопереход к следующему вопросу через 1.5 сек
        _autoAdvanceTimer?.Stop();
        _autoAdvanceTimer?.Start();
    }

    private void UpdateStats()
    {
        StatsLabel.Text = $"  |  {_trainer.CorrectAnswers}/{_trainer.TotalAnswers} ({_trainer.Accuracy:F0}%)";
    }

    private void Difficulty_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (DifficultyBox?.SelectedItem is ComboBoxItem item && item.Tag != null
            && AnswerButtons != null)
        {
            int val = int.Parse(item.Tag.ToString()!);
            if (val >= 13)
            {
                _trainer.MaxSemitones = 12;
                _trainer.IncludeUnison = true;
            }
            else
            {
                _trainer.MaxSemitones = val;
                _trainer.IncludeUnison = false;
            }
            UpdateVisibleButtons();
        }
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        _autoAdvanceTimer?.Stop();
        _trainer.ResetStats();
        UpdateStats();
        ResetButtonColors();
        _questionActive = false;
        SetButtonsEnabled(false);
        ResultLabel.Text = "Статистика сброшена. Нажми «Играть»";
        ResultLabel.Foreground = BrushDim;
        ResultBorder.Background = Brushes.Transparent;
    }
}
