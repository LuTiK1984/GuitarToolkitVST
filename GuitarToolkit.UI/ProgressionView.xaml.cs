using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GuitarToolkit.Core.DSP;
using GuitarToolkit.Core.Models;

namespace GuitarToolkit.UI;

public partial class ProgressionView : UserControl
{
    private IAudioPlayback? _audio;
    private string _selectedKey = "C";
    private int _selectedModeIndex = 0;
    private readonly List<ProgressionStep> _progression = new();
    private readonly List<Button> _keyButtons = new();
    private ProgressionStep[]? _diatonicChords;
    private int _bpm = 120;

    private static readonly Color AccentColor = Color.FromRgb(203, 166, 247);
    private static readonly Color InactiveBg = Color.FromRgb(74, 56, 96);
    private static readonly Color TextLight = Color.FromRgb(205, 214, 244);
    private static readonly Color TextDark = Color.FromRgb(26, 21, 37);
    private static readonly Color DeleteColor = Color.FromRgb(243, 139, 168);

    private bool _placeholderActive = true;

    public ProgressionView()
    {
        InitializeComponent();
        PresetNameBox.Text = "Название пресета...";
        PresetNameBox.Foreground = new SolidColorBrush(Color.FromRgb(124, 111, 150));
    }

    public void Initialize(IAudioPlayback audio)
    {
        ProgressionBuilder.LoadPresetsFromDisk();
        _audio = audio;
        BuildKeyButtons();
        BuildModeBox();
        BuildPresetButtons();
        UpdateDiatonicChords();
    }

    // ── Тональность ──────────────────────────────────────────

    private void BuildKeyButtons()
    {
        KeySelector.Items.Clear();
        _keyButtons.Clear();

        foreach (string root in ProgressionBuilder.AllRoots)
        {
            var btn = new Button
            {
                Content = root, Width = 40, Height = 30,
                FontSize = 13, FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(InactiveBg),
                Foreground = new SolidColorBrush(TextLight),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(2), Tag = root
            };
            btn.Click += (s, e) =>
            {
                _selectedKey = root;
                HighlightKeyButtons();
                UpdateDiatonicChords();
            };
            _keyButtons.Add(btn);
            KeySelector.Items.Add(btn);
        }
        HighlightKeyButtons();
    }

    private void HighlightKeyButtons()
    {
        foreach (var btn in _keyButtons)
        {
            bool active = btn.Tag?.ToString() == _selectedKey;
            btn.Background = new SolidColorBrush(active ? AccentColor : InactiveBg);
            btn.Foreground = new SolidColorBrush(active ? TextDark : TextLight);
        }
    }

    // ── Выбор лада ───────────────────────────────────────────

    private void BuildModeBox()
    {
        ModeBox.Items.Clear();
        foreach (var mode in ProgressionBuilder.AllModes)
            ModeBox.Items.Add(mode.Name);
        ModeBox.SelectedIndex = 0;
    }

    private void ModeBox_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (ModeBox.SelectedIndex >= 0)
        {
            _selectedModeIndex = ModeBox.SelectedIndex;
            UpdateDiatonicChords();
        }
    }

    // ── Диатонические аккорды ────────────────────────────────

    private void UpdateDiatonicChords()
    {
        _diatonicChords = ProgressionBuilder.GetDiatonicChords(_selectedKey, _selectedModeIndex);
        DegreeButtons.Items.Clear();

        foreach (var step in _diatonicChords)
        {
            string suffix = step.ChordType == "Major" ? "" : step.ChordType;
            var btn = new Button
            {
                Content = $"{step.Degree}\n{step.Root}{suffix}",
                Width = 75, Height = 52,
                FontSize = 12, FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(InactiveBg),
                Foreground = new SolidColorBrush(TextLight),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(3), Tag = step
            };
            btn.Click += Degree_Click;
            DegreeButtons.Items.Add(btn);
        }

        UpdateProgressionDisplay();
    }

    private void Degree_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ProgressionStep step)
        {
            _progression.Add(step);
            UpdateProgressionDisplay();
        }
    }

    // ── Пресеты ──────────────────────────────────────────────

    private void BuildPresetButtons()
    {
        PresetButtons.Items.Clear();
        foreach (var p in ProgressionBuilder.BuiltInPresets) AddPresetButton(p);
        foreach (var p in ProgressionBuilder.CustomPresets) AddPresetButton(p);
    }

    private void AddPresetButton(ProgressionPreset preset)
    {
        string prefix = preset.IsCustom ? "⭐ " : "";
        var btn = new Button
        {
            Content = prefix + preset.Name, Height = 32, FontSize = 12,
            Background = new SolidColorBrush(preset.IsCustom
                ? Color.FromRgb(45, 34, 64) : InactiveBg),
            Foreground = new SolidColorBrush(TextLight),
            BorderThickness = new Thickness(0),
            Cursor = System.Windows.Input.Cursors.Hand,
            Margin = new Thickness(3),
            Padding = new Thickness(12, 0, 12, 0),
            Tag = preset
        };
        btn.Click += Preset_Click;
        PresetButtons.Items.Add(btn);
    }

    private void Preset_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ProgressionPreset preset)
        {
            _progression.Clear();
            _progression.AddRange(
                ProgressionBuilder.BuildFromPreset(_selectedKey, preset, _selectedModeIndex));
            UpdateProgressionDisplay();
        }
    }

    // ── Сохранение пресета ───────────────────────────────────

    private void SavePreset_Click(object sender, RoutedEventArgs e)
    {
        if (_progression.Count == 0 || _diatonicChords == null) return;

        string name = _placeholderActive ? "" : PresetNameBox.Text.Trim();
        if (string.IsNullOrEmpty(name))
            name = $"Мой пресет {ProgressionBuilder.CustomPresets.Count + 1}";

        var preset = ProgressionBuilder.SaveCustomPreset(name, _progression, _diatonicChords);
        AddPresetButton(preset);

        _placeholderActive = true;
        PresetNameBox.Text = "Название пресета...";
        PresetNameBox.Foreground = new SolidColorBrush(Color.FromRgb(124, 111, 150));
    }

    private void PresetNameBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (_placeholderActive)
        {
            PresetNameBox.Text = "";
            PresetNameBox.Foreground = new SolidColorBrush(TextLight);
            _placeholderActive = false;
        }
    }

    private void PresetNameBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(PresetNameBox.Text))
        {
            PresetNameBox.Text = "Название пресета...";
            PresetNameBox.Foreground = new SolidColorBrush(Color.FromRgb(124, 111, 150));
            _placeholderActive = true;
        }
    }

    // ── Отображение прогрессии ───────────────────────────────

    private void UpdateProgressionDisplay()
    {
        ProgressionDisplay.Items.Clear();

        if (_progression.Count == 0)
        {
            ProgressionDisplay.Items.Add(new TextBlock
            {
                Text = "Пусто — добавь аккорды или выбери пресет",
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(124, 111, 150))
            });
            return;
        }

        for (int idx = 0; idx < _progression.Count; idx++)
        {
            var step = _progression[idx];
            string suffix = step.ChordType == "Major" ? "" : step.ChordType;
            int capturedIdx = idx;

            var border = new Border
            {
                Background = new SolidColorBrush(InactiveBg),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(3)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var stack = new StackPanel { Orientation = Orientation.Vertical };
            stack.Children.Add(new TextBlock
            {
                Text = step.Degree, FontSize = 10,
                Foreground = new SolidColorBrush(AccentColor),
                HorizontalAlignment = HorizontalAlignment.Center
            });
            stack.Children.Add(new TextBlock
            {
                Text = step.Root + suffix, FontSize = 16, FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(TextLight),
                HorizontalAlignment = HorizontalAlignment.Center
            });
            Grid.SetColumn(stack, 0);
            grid.Children.Add(stack);

            var delBtn = new Button
            {
                Content = "✕", Width = 18, Height = 18, FontSize = 10,
                Background = Brushes.Transparent,
                Foreground = new SolidColorBrush(DeleteColor),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(2, -2, -4, 0),
                Padding = new Thickness(0)
            };
            delBtn.Click += (s, e) =>
            {
                if (capturedIdx < _progression.Count)
                {
                    _progression.RemoveAt(capturedIdx);
                    UpdateProgressionDisplay();
                }
            };
            Grid.SetColumn(delBtn, 1);
            grid.Children.Add(delBtn);

            border.Child = grid;
            ProgressionDisplay.Items.Add(border);
        }
    }

    // ── Воспроизведение ──────────────────────────────────────

    private void Play_Click(object sender, RoutedEventArgs e)
    {
        if (_audio == null || _progression.Count == 0) return;

        int sr = _audio.SampleRate;
        double beatDuration = 60.0 / _bpm;
        int samplesPerBeat = (int)(sr * beatDuration);

        bool loop = LoopCheck.IsChecked == true;
        int repeats = loop ? 32 : 1;

        int oneCycleLen = samplesPerBeat * _progression.Count;
        float[] oneCycle = new float[oneCycleLen];

        for (int i = 0; i < _progression.Count; i++)
        {
            var step = _progression[i];
            var chord = ChordLibrary.Get(step.Root, step.ChordType);
            if (chord == null) continue;

            float[] chordSamples = ChordPlayer.Synthesize(chord, sr,
                duration: (float)beatDuration * 0.95f, strumDelay: 0.02f);

            int offset = i * samplesPerBeat;
            int len = Math.Min(chordSamples.Length, oneCycleLen - offset);
            for (int j = 0; j < len; j++)
                oneCycle[offset + j] += chordSamples[j];
        }

        float[] buffer = new float[oneCycleLen * repeats];
        for (int r = 0; r < repeats; r++)
            Array.Copy(oneCycle, 0, buffer, r * oneCycleLen, oneCycleLen);

        _audio.PlaySamples(buffer);
        StopButton.Visibility = Visibility.Visible;
    }

    private void Stop_Click(object sender, RoutedEventArgs e)
    {
        _audio?.StopPlayback();
        StopButton.Visibility = Visibility.Collapsed;
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        _progression.Clear();
        UpdateProgressionDisplay();
    }

    private void Bpm_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _bpm = (int)e.NewValue;
        if (BpmLabel != null) BpmLabel.Text = $"{_bpm} BPM";
    }
}
