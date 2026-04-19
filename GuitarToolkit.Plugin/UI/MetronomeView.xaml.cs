using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GuitarToolkit.Core.Services;

namespace GuitarToolkit.Plugin.UI;

public partial class MetronomeView : UserControl
{
    private MetronomeEngine? _metronome;
    private bool _isRunning;
    private readonly List<DateTime> _taps = new();
    private readonly List<Ellipse> _dots = new();

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

    // ── Индикаторы долей (кружки) ────────────────────────────
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
                Fill = new SolidColorBrush(Color.FromRgb(49, 50, 68)),
                Margin = new Thickness(6),
                Stroke = new SolidColorBrush(Color.FromRgb(108, 112, 134)),
                StrokeThickness = 1
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
                // Сбрасываем все кружки
                for (int i = 0; i < _dots.Count; i++)
                    _dots[i].Fill = new SolidColorBrush(Color.FromRgb(49, 50, 68));

                // Подсвечиваем текущую долю
                if (beatIndex < _dots.Count)
                {
                    _dots[beatIndex].Fill = beatIndex == 0
                        ? new SolidColorBrush(Color.FromRgb(166, 227, 161))  // акцент — зелёный
                        : new SolidColorBrush(Color.FromRgb(137, 180, 250)); // обычная — голубой
                }
            });
        }
        catch { }
    }

    // ── BPM ──────────────────────────────────────────────────
    private void BpmSlider_ValueChanged(object s, RoutedPropertyChangedEventArgs<double> e)
    {
        int bpm = (int)e.NewValue;
        if (BpmDisplay != null) BpmDisplay.Text = bpm.ToString();
        if (_metronome != null) _metronome.BPM = bpm;
    }

    // ── Громкость ────────────────────────────────────────────
    private void VolumeSlider_ValueChanged(object s, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_metronome != null) _metronome.Volume = (float)e.NewValue;
    }

    // ── Доли в такте ─────────────────────────────────────────
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

    // ── Tap Tempo ────────────────────────────────────────────
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

    // ── Старт / Стоп ─────────────────────────────────────────
    private void StartStop_Click(object s, RoutedEventArgs e)
    {
        if (_metronome == null) return;

        if (_isRunning)
        {
            _metronome.Stop();
            StartStopButton.Content = "▶  СТАРТ";
            StartStopButton.Background = new SolidColorBrush(Color.FromRgb(166, 227, 161));

            // Сбрасываем кружки
            foreach (var d in _dots)
                d.Fill = new SolidColorBrush(Color.FromRgb(49, 50, 68));
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
