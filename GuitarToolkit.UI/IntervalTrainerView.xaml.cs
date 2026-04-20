using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GuitarToolkit.Core.DSP;
using GuitarToolkit.Core.Models;

namespace GuitarToolkit.UI;

public partial class IntervalTrainerView : UserControl
{
    private IAudioPlayback? _audio;
    private readonly IntervalTrainer _trainer = new();
    private float[]? _lastSamples;
    private bool _questionActive = false;

    private static readonly Color CorrectColor = Color.FromRgb(166, 227, 161);
    private static readonly Color WrongColor = Color.FromRgb(243, 139, 168);
    private static readonly Color AccentColor = Color.FromRgb(203, 166, 247);

    public IntervalTrainerView()
    {
        InitializeComponent();
    }

    public void Initialize(IAudioPlayback audio)
    {
        _audio = audio;
        BuildAnswerButtons();
    }

    private void BuildAnswerButtons()
    {
        AnswerButtons.Items.Clear();
        UpdateVisibleButtons();
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
                Width = 110,
                Height = 52,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromRgb(74, 56, 96)),
                Foreground = new SolidColorBrush(Color.FromRgb(205, 214, 244)),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(4),
                Tag = interval.Semitones,
                IsEnabled = false
            };
            btn.Click += Answer_Click;
            AnswerButtons.Items.Add(btn);
        }
    }

    private void SetButtonsEnabled(bool enabled)
    {
        foreach (var item in AnswerButtons.Items)
        {
            if (item is Button btn)
                btn.IsEnabled = enabled;
        }
    }

    private void ResetButtonColors()
    {
        foreach (var item in AnswerButtons.Items)
        {
            if (item is Button btn)
                btn.Background = new SolidColorBrush(Color.FromRgb(74, 56, 96));
        }
    }

    // ── Играть ───────────────────────────────────────────────
    private void Play_Click(object sender, RoutedEventArgs e)
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
        ResultLabel.Foreground = new SolidColorBrush(AccentColor);
        ResultBorder.Background = new SolidColorBrush(Color.FromArgb(30, 203, 166, 247));
    }

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

        // Подсветить правильный/неправильный
        foreach (var item in AnswerButtons.Items)
        {
            if (item is Button b)
            {
                int tag = (int)b.Tag;
                if (tag == _trainer.CurrentInterval.Semitones)
                    b.Background = new SolidColorBrush(CorrectColor);
                else if (tag == answered && !correct)
                    b.Background = new SolidColorBrush(WrongColor);
            }
        }

        if (correct)
        {
            ResultLabel.Text = $"✓ Верно! {_trainer.CurrentInterval.Name}";
            ResultLabel.Foreground = new SolidColorBrush(CorrectColor);
            ResultBorder.Background = new SolidColorBrush(Color.FromArgb(30, 166, 227, 161));
        }
        else
        {
            var answered_interval = IntervalTrainer.AllIntervals.FirstOrDefault(x => x.Semitones == answered);
            ResultLabel.Text = $"✗ Нет! Было: {_trainer.CurrentInterval.Name}";
            ResultLabel.Foreground = new SolidColorBrush(WrongColor);
            ResultBorder.Background = new SolidColorBrush(Color.FromArgb(30, 243, 139, 168));
        }

        UpdateStats();
    }

    private void UpdateStats()
    {
        StatsLabel.Text = $"  |  {_trainer.CorrectAnswers}/{_trainer.TotalAnswers} ({_trainer.Accuracy:F0}%)";
    }

    // ── Настройки ────────────────────────────────────────────
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
        _trainer.ResetStats();
        UpdateStats();
        ResetButtonColors();
        _questionActive = false;
        SetButtonsEnabled(false);
        ResultLabel.Text = "Статистика сброшена. Нажми «Играть»";
        ResultLabel.Foreground = new SolidColorBrush(Color.FromRgb(124, 111, 150));
        ResultBorder.Background = Brushes.Transparent;
    }
}
