using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GuitarToolkit.Core.Models;
using GuitarToolkit.Core.Services;

namespace GuitarToolkit.UI;

public partial class TunerView : UserControl
{
    private TunerEngine? _tuner;
    private string _displayedNote = "—";

    private static readonly SolidColorBrush BrushGreen = new(Color.FromRgb(166, 227, 161));
    private static readonly SolidColorBrush BrushYellow = new(Color.FromRgb(249, 226, 175));
    private static readonly SolidColorBrush BrushRed = new(Color.FromRgb(243, 139, 168));
    private static readonly SolidColorBrush BrushAccent = new(Color.FromRgb(203, 166, 247));
    private static readonly SolidColorBrush BrushDim = new(Color.FromRgb(124, 111, 150));
    private static readonly SolidColorBrush BrushCard = new(Color.FromRgb(45, 34, 64));
    private static readonly SolidColorBrush BrushCardActive = new(Color.FromRgb(30, 60, 80));
    private static readonly SolidColorBrush BrushText = new(Color.FromRgb(205, 214, 244));
    private static readonly SolidColorBrush BrushInTuneBg = new(Color.FromRgb(30, 50, 35));
    private static readonly SolidColorBrush BrushDefaultBg = new(Color.FromRgb(52, 38, 70));

    static TunerView()
    {
        BrushGreen.Freeze(); BrushYellow.Freeze(); BrushRed.Freeze();
        BrushAccent.Freeze(); BrushDim.Freeze(); BrushCard.Freeze();
        BrushCardActive.Freeze(); BrushText.Freeze();
        BrushInTuneBg.Freeze(); BrushDefaultBg.Freeze();
    }

    public TunerView() { InitializeComponent(); }

    public void Initialize(TunerEngine tuner) => Initialize(tuner, null);

    public void Initialize(TunerEngine tuner, UserSettings? settings)
    {
        _tuner = tuner;

        foreach (var t in Tunings.All.Keys)
            TuningBox.Items.Add(t);

        if (settings != null)
        {
            TuningBox.SelectedIndex = Math.Clamp(settings.TuningIndex, 0, TuningBox.Items.Count - 1);
            GainSlider.Value = settings.TunerGainDb;
            _tuner.ReferenceA = settings.ReferenceA;
            RefLabel.Text = settings.ReferenceA.ToString("F0");
        }
        else
        {
            TuningBox.SelectedIndex = 0;
        }

        _tuner.NoteDetected += OnNoteDetected;
        _tuner.VolumeChanged += OnVolumeChanged;
    }

    public void SaveTo(UserSettings settings)
    {
        settings.TunerGainDb = (float)GainSlider.Value;
        settings.ReferenceA = _tuner?.ReferenceA ?? 440f;
        settings.TuningIndex = TuningBox.SelectedIndex;
    }

    private void OnNoteDetected(string note, float freq, float cents)
    {
        try { Dispatcher.BeginInvoke(() => UpdateUI(note, freq, cents)); }
        catch { }
    }

    private void OnVolumeChanged(float volume)
    {
        try { Dispatcher.BeginInvoke(() => UpdateVolumeBar(volume)); }
        catch { }
    }

    private void UpdateUI(string note, float freq, float cents)
    {
        FreqLabel.Text = $"{freq:F1} Hz";
        CentsLabel.Text = $"{cents:+0.0;-0.0;0} центов";

        if (note != _displayedNote && note != "—")
        {
            var fadeOut = new DoubleAnimation(1, 0.3, TimeSpan.FromMilliseconds(60));
            fadeOut.Completed += (s, e) =>
            {
                NoteLabel.Text = note;
                _displayedNote = note;
                var fadeIn = new DoubleAnimation(0.3, 1, TimeSpan.FromMilliseconds(100));
                NoteLabel.BeginAnimation(OpacityProperty, fadeIn);
            };
            NoteLabel.BeginAnimation(OpacityProperty, fadeOut);
        }
        else if (note == "—" && _displayedNote != "—")
        {
            NoteLabel.Text = "—";
            _displayedNote = "—";
        }

        double x = 170 + (cents / 50.0) * 165;
        x = Math.Clamp(x, 5, 335);

        NeedleTranslate.BeginAnimation(TranslateTransform.XProperty,
            new DoubleAnimation { To = x, Duration = TimeSpan.FromMilliseconds(80) });

        bool inTune = Math.Abs(cents) < 5;
        bool close = Math.Abs(cents) < 15;

        NeedleArrow.Fill = inTune ? BrushGreen : close ? BrushYellow : BrushRed;

        if (inTune)
        {
            InTuneLabel.Text = "✓  В СТРОЕ";
            InTuneLabel.Foreground = BrushGreen;
            InTuneIndicator.Background = BrushInTuneBg;
        }
        else
        {
            InTuneLabel.Text = cents > 0 ? "▼  Понизь" : "▲  Повысь";
            InTuneLabel.Foreground = BrushAccent;
            InTuneIndicator.Background = BrushDefaultBg;
        }

        HighlightClosestString(freq);
    }

    private void UpdateVolumeBar(float volume)
    {
        double width = Math.Clamp(volume * 640, 0, 340);
        VolumeBar.Width = width;
        VolumeBar.Fill = width < 204 ? BrushGreen : width < 289 ? BrushYellow : BrushRed;
    }

    private void GainSlider_ValueChanged(object s, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_tuner == null) return;
        _tuner.Gain = MathF.Pow(10f, (float)e.NewValue / 20f);
        if (GainLabel != null) GainLabel.Text = $"+{e.NewValue:F0} dB";
    }

    private void TuningBox_SelectionChanged(object s, SelectionChangedEventArgs e) => BuildStrings();

    private void BuildStrings()
    {
        StringsPanel.Items.Clear();
        if (TuningBox.SelectedItem == null) return;

        string key = TuningBox.SelectedItem.ToString()!;
        var strings = Tunings.All[key];

        for (int i = 0; i < strings.Length; i++)
        {
            int strNum = 6 - i;
            float strFreq = NoteUtils.NoteToFrequency(strings[i]);

            var border = new Border
            {
                Width = 56, Height = 56, Margin = new Thickness(4),
                CornerRadius = new CornerRadius(6),
                Background = BrushCard,
                Tag = strFreq,
                ToolTip = $"Струна {strNum}: {strings[i]} ({strFreq:F1} Гц)"
            };

            var stack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            stack.Children.Add(new TextBlock
            {
                Text = $"{strNum}", FontSize = 10,
                Foreground = BrushDim,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            stack.Children.Add(new TextBlock
            {
                Text = strings[i], FontSize = 16, FontWeight = FontWeights.Bold,
                Foreground = BrushText,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            border.Child = stack;
            StringsPanel.Items.Add(border);
        }
    }

    private void HighlightClosestString(float detectedFreq)
    {
        if (detectedFreq <= 0) return;

        int closestIdx = -1;
        float minDist = float.MaxValue;
        int idx = 0;

        foreach (var item in StringsPanel.Items)
        {
            if (item is Border b && b.Tag is float strFreq)
            {
                float dist = MathF.Abs(12f * MathF.Log2(detectedFreq / strFreq));
                if (dist < minDist) { minDist = dist; closestIdx = idx; }
            }
            idx++;
        }

        idx = 0;
        foreach (var item in StringsPanel.Items)
        {
            if (item is Border b)
            {
                bool match = idx == closestIdx && minDist < 1.5f;
                b.Background = match ? BrushCardActive : BrushCard;
                b.BorderThickness = new Thickness(match ? 2 : 0);
                b.BorderBrush = match ? BrushAccent : Brushes.Transparent;
            }
            idx++;
        }
    }

    private void RefUp_Click(object s, RoutedEventArgs e) => SetRef(_tuner!.ReferenceA + 1);
    private void RefDown_Click(object s, RoutedEventArgs e) => SetRef(_tuner!.ReferenceA - 1);

    private void SetRef(float value)
    {
        if (_tuner == null) return;
        _tuner.ReferenceA = Math.Clamp(value, 420, 460);
        RefLabel.Text = _tuner.ReferenceA.ToString("F0");
    }
}
