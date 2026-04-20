using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GuitarToolkit.Core.Services;

namespace GuitarToolkit.UI;

public partial class MetronomeView : UserControl
{
    private MetronomeEngine? _metronome;
    private bool _isRunning;
    private readonly List<DateTime> _taps = new();
    private readonly List<Ellipse> _dots = new();

    private static readonly SolidColorBrush BrushDotOff = new(Color.FromRgb(45, 34, 64));
    private static readonly SolidColorBrush BrushAccent = new(Color.FromRgb(166, 227, 161));
    private static readonly SolidColorBrush BrushNormal = new(Color.FromRgb(203, 166, 247));
    private static readonly SolidColorBrush BrushStroke = new(Color.FromRgb(124, 111, 150));

    static MetronomeView()
    {
        BrushDotOff.Freeze(); BrushAccent.Freeze();
        BrushNormal.Freeze(); BrushStroke.Freeze();
    }

    public MetronomeView()
    {
        InitializeComponent();
    }

    public void Initialize(MetronomeEngine metronome)
    {
        _metronome = metronome;
        _metronome.BeatTick += OnBeat;
        BuildBeatDots();
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
                            Color.FromRgb(45, 34, 64),
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
                        ? Color.FromRgb(166, 227, 161)
                        : Color.FromRgb(203, 166, 247);
                    _dots[beatIndex].Fill = new SolidColorBrush(targetColor);
                }

                // Пульсация BPM-числа
                var pulse = new DoubleAnimation(1.08, 1.0, TimeSpan.FromMilliseconds(150));
                BpmScale.BeginAnimation(ScaleTransform.ScaleXProperty, pulse);
                BpmScale.BeginAnimation(ScaleTransform.ScaleYProperty, pulse);
            });
        }
        catch { }
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
            StartStopButton.Background = new SolidColorBrush(Color.FromRgb(166, 227, 161));

            foreach (var d in _dots)
                d.Fill = BrushDotOff;
        }
        else
        {
            _metronome.BPM = (int)BpmSlider.Value;
            _metronome.Volume = (float)VolumeSlider.Value;
            _metronome.Start();
            StartStopButton.Content = "⏹  СТОП";
            StartStopButton.Background = new SolidColorBrush(Color.FromRgb(243, 139, 168));
        }

        _isRunning = !_isRunning;
    }
}
