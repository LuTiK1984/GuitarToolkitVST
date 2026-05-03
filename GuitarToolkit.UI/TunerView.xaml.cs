using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GuitarToolkit.Core.Models;
using GuitarToolkit.Core.Services;

namespace GuitarToolkit.UI;

public partial class TunerView : UserControl, IThemeAware
{
    private TunerEngine? _tuner;
    private string _displayedNote = "\u2014";
    private bool _isUpdatingInputDevices;

    private static SolidColorBrush BrushGreen => ThemeManager.GetBrush("GoodBrush");
    private static SolidColorBrush BrushYellow => ThemeManager.GetBrush("WarnBrush");
    private static SolidColorBrush BrushRed => ThemeManager.GetBrush("DangerBrush");
    private static SolidColorBrush BrushAccent => ThemeManager.GetBrush("AccentBrush");
    private static SolidColorBrush BrushDim => ThemeManager.GetBrush("MutedTextBrush");
    private static SolidColorBrush BrushCard => ThemeManager.GetBrush("ControlBrush");
    private static SolidColorBrush BrushCardActive => ThemeManager.GetBrush("ControlHoverBrush");
    private static SolidColorBrush BrushText => ThemeManager.GetBrush("TextBrush");
    private static SolidColorBrush BrushInTuneBg => ThemeManager.GetBrush("DarkBrush");
    private static SolidColorBrush BrushDefaultBg => ThemeManager.GetBrush("ControlBrush");

    public TunerView() { InitializeComponent(); }

    public void ApplyTheme()
    {
        SetInputStatus(InputStatusLabel?.Text ?? "", InputStatusLabel?.Text?.Contains("\u0417\u0430\u043F\u0438\u0441\u044C") == true);
        BuildStrings();
    }

    public event EventHandler<int>? InputDeviceSelected;

    public int SelectedInputDeviceIndex => InputDeviceBox?.SelectedIndex ?? -1;

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

    public void SetInputDevices(IReadOnlyList<string> devices, int selectedIndex)
    {
        if (InputDeviceBox == null) return;

        _isUpdatingInputDevices = true;
        try
        {
            InputDeviceBox.Items.Clear();

            if (devices.Count == 0)
            {
                InputDeviceBox.Items.Add("\u0412\u0445\u043E\u0434 \u0443\u043F\u0440\u0430\u0432\u043B\u044F\u0435\u0442\u0441\u044F \u0445\u043E\u0441\u0442\u043E\u043C");
                InputDeviceBox.SelectedIndex = 0;
                InputDeviceBox.IsEnabled = false;
                SetInputStatus("\u041D\u0435\u0442 \u0443\u0441\u0442\u0440\u043E\u0439\u0441\u0442\u0432", false);
                return;
            }

            foreach (var device in devices)
                InputDeviceBox.Items.Add(device);

            InputDeviceBox.IsEnabled = true;
            InputDeviceBox.SelectedIndex = Math.Clamp(selectedIndex, 0, devices.Count - 1);
        }
        finally
        {
            _isUpdatingInputDevices = false;
        }
    }

    public void SetInputStatus(string text, bool isActive)
    {
        if (InputStatusLabel == null) return;
        InputStatusLabel.Text = text;
        InputStatusLabel.Foreground = isActive ? BrushGreen : BrushDim;
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
        CentsLabel.Text = $"{cents:+0.0;-0.0;0} \u0446\u0435\u043D\u0442\u043E\u0432";

        if (note != _displayedNote && !IsNoNote(note))
        {
            NoteLabel.Text = note;
            _displayedNote = note;
            NoteLabel.Opacity = 1;
        }
        else if (IsNoNote(note) && _displayedNote != "\u2014")
        {
            NoteLabel.Text = "\u2014";
            _displayedNote = "\u2014";
        }

        double x = 170 + (cents / 50.0) * 158;
        x = Math.Clamp(x, 8, 332);

        NeedleTranslate.BeginAnimation(TranslateTransform.XProperty,
            new DoubleAnimation { To = x, Duration = TimeSpan.FromMilliseconds(110) });

        bool inTune = Math.Abs(cents) < 5;
        bool close = Math.Abs(cents) < 15;

        NeedleArrow.Fill = inTune ? BrushGreen : close ? BrushYellow : BrushRed;

        if (inTune)
        {
            InTuneLabel.Text = "\u2713 \u0412 \u0441\u0442\u0440\u043E\u044E";
            InTuneLabel.Foreground = BrushGreen;
            InTuneIndicator.Background = BrushInTuneBg;
        }
        else
        {
            InTuneLabel.Text = cents > 0
                ? "\u25BC \u041F\u043E\u043D\u0438\u0437\u044C"
                : "\u25B2 \u041F\u043E\u0432\u044B\u0441\u044C";
            InTuneLabel.Foreground = BrushAccent;
            InTuneIndicator.Background = BrushDefaultBg;
        }

        HighlightClosestString(freq);
    }

    private void UpdateVolumeBar(float volume)
    {
        double maxWidth = VolumeBar.Parent is FrameworkElement parent && parent.ActualWidth > 0
            ? parent.ActualWidth
            : 340;
        double width = Math.Clamp(volume * maxWidth * 1.9, 0, maxWidth);
        VolumeBar.Width = width;
        VolumeBar.Fill = width < maxWidth * 0.6 ? BrushGreen : width < maxWidth * 0.85 ? BrushYellow : BrushRed;
        VolumeBar.Opacity = ThemeManager.CurrentTheme == ToolkitTheme.Light ? 0.68 : 1.0;
    }

    private void GainSlider_ValueChanged(object s, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_tuner == null) return;
        _tuner.Gain = MathF.Pow(10f, (float)e.NewValue / 20f);
        if (GainLabel != null) GainLabel.Text = $"+{e.NewValue:F0} dB";
    }

    private void TuningBox_SelectionChanged(object s, SelectionChangedEventArgs e) => BuildStrings();

    private void InputDeviceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isUpdatingInputDevices || InputDeviceBox.SelectedIndex < 0 || !InputDeviceBox.IsEnabled)
            return;

        InputDeviceSelected?.Invoke(this, InputDeviceBox.SelectedIndex);
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
            float strFreq = NoteUtils.NoteToFrequency(strings[i]);

            var border = new Border
            {
                Height = 46,
                Margin = new Thickness(3, 0, 3, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                CornerRadius = new CornerRadius(4),
                Background = BrushCard,
                Tag = strFreq,
                ToolTip = $"\u0421\u0442\u0440\u0443\u043D\u0430 {strNum}: {strings[i]} ({strFreq:F1} Hz)"
            };

            var stack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            stack.Children.Add(new TextBlock
            {
                Text = $"{strNum}",
                FontSize = 10,
                Foreground = BrushDim,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            stack.Children.Add(new TextBlock
            {
                Text = strings[i],
                FontSize = 16,
                FontWeight = FontWeights.Bold,
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

    private static bool IsNoNote(string note) => note == "\u2014";
}
