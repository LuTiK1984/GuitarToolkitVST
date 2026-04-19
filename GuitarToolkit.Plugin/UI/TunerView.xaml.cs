using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GuitarToolkit.Core.Models;
using GuitarToolkit.Core.Services;

namespace GuitarToolkit.Plugin.UI;

public partial class TunerView : UserControl
{
    private TunerEngine? _tuner;

    public TunerView()
    {
        InitializeComponent();
    }

    public void Initialize(TunerEngine tuner)
    {
        _tuner = tuner;

        // Заполняем ComboBox строёв
        foreach (var t in Tunings.All.Keys)
            TuningBox.Items.Add(t);
        TuningBox.SelectedIndex = 0;

        // Подписка на события движка (приходят из аудиопотока!)
        _tuner.NoteDetected += OnNoteDetected;
        _tuner.VolumeChanged += OnVolumeChanged;
    }

    // ── События от движка ────────────────────────────────────
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

    // ── Обновление UI ────────────────────────────────────────
    private void UpdateUI(string note, float freq, float cents)
    {
        NoteLabel.Text = note;
        FreqLabel.Text = $"{freq:F1} Hz";
        CentsLabel.Text = $"{cents:+0.0;-0.0;0} центов";

        // Стрелка: cents от -50 до +50, ширина шкалы 320
        double x = 160 + (cents / 50.0) * 155;
        x = Math.Clamp(x, 5, 315);

        var anim = new DoubleAnimation
        {
            To = x,
            Duration = TimeSpan.FromMilliseconds(80)
        };
        NeedleTranslate.BeginAnimation(TranslateTransform.XProperty, anim);

        // Цвет стрелки
        bool inTune = Math.Abs(cents) < 5;
        bool close = Math.Abs(cents) < 15;

        NeedleArrow.Fill = inTune
            ? new SolidColorBrush(Color.FromRgb(166, 227, 161))    // зелёный
            : close
                ? new SolidColorBrush(Color.FromRgb(249, 226, 175)) // жёлтый
                : new SolidColorBrush(Color.FromRgb(243, 139, 168)); // красный

        if (inTune)
        {
            InTuneLabel.Text = "✓  В СТРОЕ";
            InTuneLabel.Foreground = new SolidColorBrush(Color.FromRgb(166, 227, 161));
            InTuneIndicator.Background = new SolidColorBrush(Color.FromRgb(30, 50, 35));
        }
        else
        {
            InTuneLabel.Text = cents > 0 ? "▼  Понизь" : "▲  Повысь";
            InTuneLabel.Foreground = new SolidColorBrush(Color.FromRgb(203, 166, 247));
            InTuneIndicator.Background = new SolidColorBrush(Color.FromRgb(52, 38, 70));
        }

        HighlightClosestString(note);
    }

    private void UpdateVolumeBar(float volume)
    {
        double width = Math.Clamp(volume * 600, 0, 320);
        VolumeBar.Width = width;

        if (width < 192)
            VolumeBar.Fill = new SolidColorBrush(Color.FromRgb(166, 227, 161));
        else if (width < 272)
            VolumeBar.Fill = new SolidColorBrush(Color.FromRgb(249, 226, 175));
        else
            VolumeBar.Fill = new SolidColorBrush(Color.FromRgb(243, 139, 168));
    }

    // ── Усиление ─────────────────────────────────────────────
    private void GainSlider_ValueChanged(object s, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_tuner == null) return;
        float linear = MathF.Pow(10f, (float)e.NewValue / 20f);
        _tuner.Gain = linear;
        if (GainLabel != null) GainLabel.Text = $"+{e.NewValue:F0} dB";
    }

    // ── Строй ────────────────────────────────────────────────
    private void TuningBox_SelectionChanged(object s, SelectionChangedEventArgs e)
    {
        BuildStrings();
    }

    private void BuildStrings()
    {
        StringsPanel.Items.Clear();
        if (TuningBox.SelectedItem == null) return;

        string key = TuningBox.SelectedItem.ToString()!;
        var strings = Tunings.All[key];

        for (int i = 0; i < strings.Length; i++)
        {
            int strNum = 6 - i;
            var border = new Border
            {
                Width = 56, Height = 56, Margin = new Thickness(4),
                CornerRadius = new CornerRadius(6),
                Background = new SolidColorBrush(Color.FromRgb(45, 34, 64)),
                Tag = strings[i]
            };

            var stack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            stack.Children.Add(new TextBlock
            {
                Text = $"{strNum}", FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(124, 111, 150)),
                HorizontalAlignment = HorizontalAlignment.Center
            });
            stack.Children.Add(new TextBlock
            {
                Text = strings[i], FontSize = 16, FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(205, 214, 244)),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            border.Child = stack;
            StringsPanel.Items.Add(border);
        }
    }

    private void HighlightClosestString(string detectedNote)
    {
        foreach (var item in StringsPanel.Items)
        {
            if (item is not Border b) continue;

            string strNoteName = NoteUtils.StripOctave(b.Tag?.ToString() ?? "");
            bool match = strNoteName == detectedNote;

            b.Background = match
                ? new SolidColorBrush(Color.FromRgb(30, 60, 80))
                : new SolidColorBrush(Color.FromRgb(45, 34, 64));
            b.BorderThickness = new Thickness(match ? 2 : 0);
            b.BorderBrush = match
                ? new SolidColorBrush(Color.FromRgb(203, 166, 247))
                : Brushes.Transparent;
        }
    }

    // ── Эталон ───────────────────────────────────────────────
    private void RefUp_Click(object s, RoutedEventArgs e) => SetRef(_tuner!.ReferenceA + 1);
    private void RefDown_Click(object s, RoutedEventArgs e) => SetRef(_tuner!.ReferenceA - 1);

    private void SetRef(float value)
    {
        if (_tuner == null) return;
        _tuner.ReferenceA = Math.Clamp(value, 420, 460);
        RefLabel.Text = _tuner.ReferenceA.ToString("F0");
    }
}
