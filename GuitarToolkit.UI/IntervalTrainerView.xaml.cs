using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using GuitarToolkit.Core.DSP;
using GuitarToolkit.Core.Models;

namespace GuitarToolkit.UI;

public partial class IntervalTrainerView : UserControl, IThemeAware
{
    private IAudioPlayback? _audio;
    private readonly IntervalTrainer _trainer = new();
    private float[]? _lastSamples;
    private bool _questionActive = false;
    private DispatcherTimer? _autoAdvanceTimer;

    private static SolidColorBrush BrushCorrect => ThemeManager.GetBrush("GoodBrush");
    private static SolidColorBrush BrushWrong => ThemeManager.GetBrush("DangerBrush");
    private static SolidColorBrush BrushAccent => ThemeManager.GetBrush("AccentBrush");
    private static SolidColorBrush BrushBtn => ThemeManager.GetBrush("ControlBrush");
    private static SolidColorBrush BrushButtonBorder => ThemeManager.GetBrush("PanelBorderBrush");
    private static SolidColorBrush BrushPanelDark => ThemeManager.GetBrush("DarkBrush");
    private static SolidColorBrush BrushText => ThemeManager.GetBrush("TextBrush");
    private static SolidColorBrush BrushDim => ThemeManager.GetBrush("MutedTextBrush");

    public IntervalTrainerView()
    {
        InitializeComponent();
    }

    public void ApplyTheme()
    {
        ResetButtonColors();
        if (!_questionActive)
        {
            ResultLabel.Foreground = BrushDim;
            ResultBorder.Background = BrushPanelDark;
        }
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
                Width = 112, Height = 50,
                FontSize = 12, FontWeight = FontWeights.Bold,
                Background = BrushBtn, Foreground = BrushText,
                BorderBrush = BrushButtonBorder,
                BorderThickness = new Thickness(1),
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 0, 6, 6),
                Padding = new Thickness(8, 0, 8, 0),
                Style = (Style)FindResource("TrainerButton"),
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
        {
            if (item is Button btn)
            {
                btn.Background = BrushBtn;
                btn.BorderBrush = BrushButtonBorder;
                btn.Foreground = BrushText;
            }
        }
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
        ResultBorder.Background = AlphaBrush("AccentBrush", 30);
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
                {
                    b.Background = BrushCorrect;
                    b.BorderBrush = BrushCorrect;
                    b.Foreground = BrushPanelDark;
                }
                else if (tag == answered && !correct)
                {
                    b.Background = BrushWrong;
                    b.BorderBrush = BrushWrong;
                    b.Foreground = BrushPanelDark;
                }
            }
        }

        if (correct)
        {
            ResultLabel.Text = $"✓ Верно! {_trainer.CurrentInterval.Name}";
            ResultLabel.Foreground = BrushCorrect;
            ResultBorder.Background = AlphaBrush("GoodBrush", 30);
        }
        else
        {
            ResultLabel.Text = $"✗ Нет! Было: {_trainer.CurrentInterval.Name}";
            ResultLabel.Foreground = BrushWrong;
            ResultBorder.Background = AlphaBrush("DangerBrush", 30);
        }

        UpdateStats();

        // Автопереход к следующему вопросу через 1.5 сек
        _autoAdvanceTimer?.Stop();
        _autoAdvanceTimer?.Start();
    }

    private void UpdateStats()
    {
        StatsLabel.Text = $"{_trainer.CorrectAnswers}/{_trainer.TotalAnswers} ({_trainer.Accuracy:F0}%)";
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
        ResultBorder.Background = BrushPanelDark;
    }

    private static SolidColorBrush AlphaBrush(string resourceName, byte alpha)
    {
        Color color = ThemeManager.GetColor(resourceName);
        color.A = alpha;
        return new SolidColorBrush(color);
    }
}
