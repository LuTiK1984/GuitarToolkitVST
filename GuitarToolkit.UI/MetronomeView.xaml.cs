using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GuitarToolkit.Core.Services;

namespace GuitarToolkit.UI;

public partial class MetronomeView : UserControl, IThemeAware
{
    private MetronomeEngine? _metronome;
    private bool _isRunning;
    private bool _pendulumRight;
    private readonly List<DateTime> _taps = new();
    private readonly List<Ellipse> _dots = new();

    private static SolidColorBrush BrushDotOff => ThemeManager.GetBrush("ControlBrush");
    private static SolidColorBrush BrushAccent => ThemeManager.GetBrush("GoodBrush");
    private static SolidColorBrush BrushNormal => ThemeManager.GetBrush("AccentBrush");
    private static SolidColorBrush BrushStroke => ThemeManager.GetBrush("MutedTextBrush");

    public MetronomeView()
    {
        InitializeComponent();
    }

    public void ApplyTheme()
    {
        BuildBeatDots();
        UpdateStartStopVisual();
    }

    public void SaveTo(UserSettings settings)
    {
        settings.BPM = (int)BpmSlider.Value;
        settings.BeatsPerMeasure = _metronome?.BeatsPerMeasure ?? 4;
        settings.MetronomeVolume = (float)VolumeSlider.Value;
    }

    public void ToggleStartStop()
    {
        StartStop_Click(this, new RoutedEventArgs());
    }

    public void Initialize(MetronomeEngine metronome) => Initialize(metronome, null);

    public void Initialize(MetronomeEngine metronome, UserSettings? settings)
    {
        _metronome = metronome;
        _metronome.BeatTick += OnBeat;

        if (settings != null)
        {
            BpmSlider.Value = settings.BPM;
            VolumeSlider.Value = settings.MetronomeVolume;
            SetBeats(settings.BeatsPerMeasure);
        }
        else
        {
            BuildBeatDots();
        }
    }

    private void BuildBeatDots()
    {
        BeatIndicators.Items.Clear();
        _dots.Clear();

        int beats = _metronome?.BeatsPerMeasure ?? 4;
        for (int i = 0; i < beats; i++)
        {
            var dot = new Ellipse
            {
                Width = 28, Height = 28,
                Fill = BrushDotOff,
                Margin = new Thickness(6),
                Stroke = BrushStroke,
                StrokeThickness = 1,
                ToolTip = i == 0 ? "Сильная доля" : $"Доля {i + 1}"
            };
            _dots.Add(dot);
            BeatIndicators.Items.Add(dot);
        }
    }

    private void OnBeat(int beatIndex)
    {
        try
        {
            Dispatcher.BeginInvoke(() =>
            {
                // Плавное затухание всех кружков
                for (int i = 0; i < _dots.Count; i++)
                {
                    if (i != beatIndex)
                    {
                        var fadeOut = new ColorAnimation(
                            ThemeManager.GetColor("ControlBrush"),
                            TimeSpan.FromMilliseconds(200));
                        _dots[i].Fill = new SolidColorBrush();
                        ((SolidColorBrush)_dots[i].Fill).BeginAnimation(
                            SolidColorBrush.ColorProperty, fadeOut);
                    }
                }

                // Текущая доля — загорается мгновенно
                if (beatIndex < _dots.Count)
                {
                    Color targetColor = beatIndex == 0
                        ? ThemeManager.GetColor("GoodBrush")
                        : ThemeManager.GetColor("AccentBrush");
                    _dots[beatIndex].Fill = new SolidColorBrush(targetColor);
                }

                // Пульсация BPM-числа
                var pulse = new DoubleAnimation(1.08, 1.0, TimeSpan.FromMilliseconds(150));
                BpmScale.BeginAnimation(ScaleTransform.ScaleXProperty, pulse);
                BpmScale.BeginAnimation(ScaleTransform.ScaleYProperty, pulse);

                _pendulumRight = !_pendulumRight;
                var swing = new DoubleAnimation(
                    _pendulumRight ? 18d : -18d,
                    TimeSpan.FromMilliseconds(GetBeatAnimationMilliseconds()))
                {
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
                };
                PendulumRotate.BeginAnimation(RotateTransform.AngleProperty, swing);

                var bobPulse = new DoubleAnimation(1.12, 1.0, TimeSpan.FromMilliseconds(140));
                PendulumBobScale.BeginAnimation(ScaleTransform.ScaleXProperty, bobPulse);
                PendulumBobScale.BeginAnimation(ScaleTransform.ScaleYProperty, bobPulse);
            });
        }
        catch { }
    }

    private int GetBeatAnimationMilliseconds()
    {
        int bpm = _metronome?.BPM ?? (int)(BpmSlider?.Value ?? 120d);
        return Math.Clamp((int)(60000d / Math.Max(30, bpm) * 0.92d), 140, 620);
    }

    private void BpmSlider_ValueChanged(object s, RoutedPropertyChangedEventArgs<double> e)
    {
        int bpm = (int)e.NewValue;
        if (BpmDisplay != null) BpmDisplay.Text = bpm.ToString();
        if (_metronome != null) _metronome.BPM = bpm;
    }

    private void VolumeSlider_ValueChanged(object s, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_metronome != null) _metronome.Volume = (float)e.NewValue;
        if (VolumeLabel != null) VolumeLabel.Text = $"{(int)Math.Round(e.NewValue * 100d)}%";
    }

    private void IncrBeats_Click(object s, RoutedEventArgs e) => SetBeats((_metronome?.BeatsPerMeasure ?? 4) + 1);
    private void DecrBeats_Click(object s, RoutedEventArgs e) => SetBeats((_metronome?.BeatsPerMeasure ?? 4) - 1);

    private void SetBeats(int value)
    {
        if (_metronome == null) return;
        value = Math.Clamp(value, 2, 8);
        _metronome.BeatsPerMeasure = value;
        BeatsLabel.Text = value.ToString();
        BuildBeatDots();
    }

    private void TapButton_Click(object s, RoutedEventArgs e)
    {
        var now = DateTime.Now;
        _taps.Add(now);

        if (_taps.Count > 1 && (now - _taps[^2]).TotalSeconds > 3)
            _taps.Clear();

        if (_taps.Count >= 2)
        {
            double sum = 0;
            for (int i = 1; i < _taps.Count; i++)
                sum += (_taps[i] - _taps[i - 1]).TotalMilliseconds;

            int bpm = (int)(60000 / (sum / (_taps.Count - 1)));
            bpm = Math.Clamp(bpm, 30, 300);
            BpmSlider.Value = bpm;
        }

        if (_taps.Count > 8) _taps.RemoveAt(0);
    }

    private void StartStop_Click(object s, RoutedEventArgs e)
    {
        if (_metronome == null) return;

        if (_isRunning)
        {
            _metronome.Stop();
            StartStopButton.Content = "▶  СТАРТ";
            StartStopButton.Background = ThemeManager.GetBrush("GoodBrush");

            foreach (var d in _dots)
                d.Fill = BrushDotOff;
        }
        else
        {
            _metronome.BPM = (int)BpmSlider.Value;
            _metronome.Volume = (float)VolumeSlider.Value;
            _metronome.Start();
            StartStopButton.Content = "⏹  СТОП";
            StartStopButton.Background = ThemeManager.GetBrush("DangerBrush");
        }

        _isRunning = !_isRunning;
        UpdateStartStopVisual();
    }

    private void UpdateStartStopVisual()
    {
        if (_isRunning)
        {
            StartStopButton.Content = "■ Стоп";
            StartStopButton.Background = ThemeManager.GetBrush("DangerBrush");
            StartStopButton.BorderBrush = ThemeManager.GetBrush("DangerBrush");
        }
        else
        {
            StartStopButton.Content = "▶ Старт";
            StartStopButton.Background = ThemeManager.GetBrush("GoodBrush");
            StartStopButton.BorderBrush = ThemeManager.GetBrush("GoodBrush");
            PendulumRotate.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation(0d, TimeSpan.FromMilliseconds(160)));
        }
    }

    private void TempoPreset_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string value } && int.TryParse(value, out int bpm))
        {
            BpmSlider.Value = Math.Clamp(bpm, (int)BpmSlider.Minimum, (int)BpmSlider.Maximum);
        }
    }
}
