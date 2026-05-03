using GuitarToolkit.Core.DSP;
using GuitarToolkit.Core.Models;
using GuitarToolkit.Core.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GuitarToolkit.UI;

public partial class ProgressionView : UserControl, IThemeAware
{
    private IAudioPlayback? _audio;
    private string _selectedKey = "C";
    private int _selectedModeIndex;
    private readonly List<ProgressionStep> _progression = new();
    private readonly List<Button> _keyButtons = new();
    private ProgressionStep[]? _diatonicChords;
    private int _bpm = 120;
    private bool _placeholderActive = true;

    private static Color AccentColor => ThemeManager.GetColor("AccentBrush");
    private static Color InactiveBg => ThemeManager.GetColor("ControlBrush");
    private static Color InactiveBorder => ThemeManager.GetColor("PanelBorderBrush");
    private static Color DarkBg => ThemeManager.GetColor("DarkBrush");
    private static Color TextLight => ThemeManager.GetColor("TextBrush");
    private static Color TextDark => ThemeManager.GetColor("DarkBrush");
    private static Color MutedText => ThemeManager.GetColor("MutedTextBrush");
    private static Color DeleteColor => ThemeManager.GetColor("DangerBrush");

    public ProgressionView()
    {
        InitializeComponent();
        ResetPresetNamePlaceholder();
        SaveStatusText.Text = "Введите имя, чтобы сохранить текущую прогрессию.";
    }

    public void Initialize(IAudioPlayback audio) => Initialize(audio, null);

    public void ApplyTheme()
    {
        HighlightKeyButtons();
        UpdateDiatonicChords();
        PresetNameBox.Foreground = new SolidColorBrush(_placeholderActive ? MutedText : TextLight);
    }

    public void Initialize(IAudioPlayback audio, UserSettings? settings)
    {
        _audio = audio;
        ProgressionBuilder.LoadPresetsFromDisk();
        BuildKeyButtons();
        BuildModeBox();
        BuildPresetButtons();

        if (settings != null)
        {
            _selectedKey = settings.LastProgressionKey;
            HighlightKeyButtons();
            ModeBox.SelectedIndex = Math.Clamp(settings.LastModeIndex, 0, ModeBox.Items.Count - 1);
            BpmSlider.Value = settings.ProgressionBPM;
        }

        UpdateDiatonicChords();
    }

    public void SaveTo(UserSettings settings)
    {
        settings.LastProgressionKey = _selectedKey;
        settings.LastModeIndex = ModeBox.SelectedIndex;
        settings.ProgressionBPM = (int)BpmSlider.Value;
    }

    private void BuildKeyButtons()
    {
        KeySelector.Items.Clear();
        _keyButtons.Clear();

        foreach (string root in ProgressionBuilder.AllRoots)
        {
            var btn = CreateButton(root, width: 40);
            btn.Tag = root;
            btn.Click += (_, _) =>
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
            btn.BorderBrush = new SolidColorBrush(active ? AccentColor : InactiveBg);
            btn.Foreground = new SolidColorBrush(active ? TextDark : TextLight);
        }
    }

    private void BuildModeBox()
    {
        ModeBox.Items.Clear();
        foreach (var mode in ProgressionBuilder.AllModes)
        {
            ModeBox.Items.Add(mode.Name);
        }

        ModeBox.SelectedIndex = 0;
    }

    private void ModeBox_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (ModeBox.SelectedIndex < 0)
            return;

        _selectedModeIndex = ModeBox.SelectedIndex;
        UpdateDiatonicChords();
    }

    private void UpdateDiatonicChords()
    {
        _diatonicChords = ProgressionBuilder.GetDiatonicChords(_selectedKey, _selectedModeIndex);
        DegreeButtons.Items.Clear();

        foreach (var step in _diatonicChords)
        {
            string suffix = step.ChordType == "Major" ? "" : step.ChordType;
            var btn = CreateButton($"{step.Degree}\n{step.Root}{suffix}", width: 68, height: 44);
            btn.Tag = step;
            btn.Click += Degree_Click;
            DegreeButtons.Items.Add(btn);
        }

        UpdateProgressionDisplay();
    }

    private void Degree_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: ProgressionStep step })
            return;

        _progression.Add(step);
        UpdateProgressionDisplay();
    }

    private void BuildPresetButtons()
    {
        BuiltInPresetBox.Items.Clear();
        CustomPresetBox.Items.Clear();

        foreach (var preset in ProgressionBuilder.BuiltInPresets)
        {
            BuiltInPresetBox.Items.Add(new PresetListItem(preset));
        }

        foreach (var preset in ProgressionBuilder.CustomPresets)
        {
            CustomPresetBox.Items.Add(new PresetListItem(preset));
        }

        BuiltInPresetBox.SelectedIndex = BuiltInPresetBox.Items.Count > 0 ? 0 : -1;
        CustomPresetBox.SelectedIndex = CustomPresetBox.Items.Count > 0 ? 0 : -1;

        bool hasSavedPresets = CustomPresetBox.Items.Count > 0;
        LoadSavedPresetButton.IsEnabled = hasSavedPresets;
        DeleteSavedPresetButton.IsEnabled = hasSavedPresets;
    }

    private void AddPresetButton(ProgressionPreset preset, ItemsControl target)
    {
        if (!preset.IsCustom)
        {
            var btn = CreateButton(preset.Name);
            btn.Padding = new Thickness(10, 0, 10, 0);
            btn.Tag = preset;
            btn.Click += Preset_Click;
            target.Items.Add(btn);
            return;
        }

        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 0, 6, 6)
        };

        var presetButton = CreateButton(preset.Name);
        presetButton.Background = new SolidColorBrush(DarkBg);
        presetButton.BorderBrush = new SolidColorBrush(InactiveBg);
        presetButton.Padding = new Thickness(10, 0, 8, 0);
        presetButton.Margin = new Thickness(0);
        presetButton.Tag = preset;
        presetButton.Click += Preset_Click;

        var deleteButton = CreateButton("×", width: 26);
        deleteButton.Background = new SolidColorBrush(DarkBg);
        deleteButton.BorderBrush = new SolidColorBrush(InactiveBg);
        deleteButton.Foreground = new SolidColorBrush(DeleteColor);
        deleteButton.Margin = new Thickness(0);
        deleteButton.Tag = preset;
        deleteButton.Click += DeletePreset_Click;

        panel.Children.Add(presetButton);
        panel.Children.Add(deleteButton);
        target.Items.Add(panel);
    }

    private void Preset_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: ProgressionPreset preset })
            return;

        _progression.Clear();
        _progression.AddRange(ProgressionBuilder.BuildFromPreset(_selectedKey, preset, _selectedModeIndex));
        UpdateProgressionDisplay();
    }

    private void DeletePreset_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: ProgressionPreset preset })
            return;

        ProgressionBuilder.CustomPresets.Remove(preset);
        ProgressionBuilder.SavePresetsToDisk();
        SaveStatusText.Text = $"Удалено: {preset.Name}";
        BuildPresetButtons();
    }

    private void LoadBuiltInPreset_Click(object sender, RoutedEventArgs e)
    {
        if (BuiltInPresetBox.SelectedItem is not PresetListItem item)
            return;

        LoadPreset(item.Preset);
    }

    private void LoadCustomPreset_Click(object sender, RoutedEventArgs e)
    {
        if (CustomPresetBox.SelectedItem is not PresetListItem item)
            return;

        LoadPreset(item.Preset);
    }

    private void DeleteSelectedPreset_Click(object sender, RoutedEventArgs e)
    {
        if (CustomPresetBox.SelectedItem is not PresetListItem item)
            return;

        var preset = item.Preset;
        ProgressionBuilder.CustomPresets.Remove(preset);
        ProgressionBuilder.SavePresetsToDisk();
        SaveStatusText.Text = $"Удалено: {preset.Name}";
        BuildPresetButtons();
    }

    private void LoadPreset(ProgressionPreset preset)
    {
        _progression.Clear();
        _progression.AddRange(ProgressionBuilder.BuildFromPreset(_selectedKey, preset, _selectedModeIndex));
        UpdateProgressionDisplay();
    }

    private void SavePreset_Click(object sender, RoutedEventArgs e)
    {
        if (_progression.Count == 0 || _diatonicChords == null)
        {
            SaveStatusText.Text = "Нечего сохранять: сначала соберите прогрессию.";
            return;
        }

        string name = _placeholderActive ? "" : PresetNameBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            SaveStatusText.Text = "Введите имя пресета перед сохранением.";
            return;
        }

        var existing = ProgressionBuilder.CustomPresets
            .FirstOrDefault(p => p.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        if (existing != null)
        {
            ProgressionBuilder.CustomPresets.Remove(existing);
        }

        ProgressionBuilder.SaveCustomPreset(name, _progression, _diatonicChords);
        BuildPresetButtons();

        SaveStatusText.Text = existing == null
            ? $"Сохранено: {name}"
            : $"Обновлено: {name}";
        ResetPresetNamePlaceholder();
    }

    private void PresetNameBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (!_placeholderActive)
            return;

        PresetNameBox.Text = "";
        PresetNameBox.Foreground = new SolidColorBrush(TextLight);
        _placeholderActive = false;
    }

    private void PresetNameBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(PresetNameBox.Text))
        {
            ResetPresetNamePlaceholder();
        }
    }

    private void ResetPresetNamePlaceholder()
    {
        _placeholderActive = true;
        PresetNameBox.Text = "Название пресета...";
        PresetNameBox.Foreground = new SolidColorBrush(MutedText);
    }

    private void UpdateProgressionDisplay()
    {
        ProgressionDisplay.Items.Clear();

        if (_progression.Count == 0)
        {
            ProgressionDisplay.Items.Add(new TextBlock
            {
                Text = "Пусто: добавьте аккорды слева или выберите пресет.",
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(MutedText)
            });
            return;
        }

        for (int index = 0; index < _progression.Count; index++)
        {
            var step = _progression[index];
            string suffix = step.ChordType == "Major" ? "" : step.ChordType;
            int capturedIndex = index;

            var border = new Border
            {
                Background = new SolidColorBrush(InactiveBg),
                BorderBrush = new SolidColorBrush(InactiveBorder),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(0, 0, 6, 6),
                MinWidth = 48
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var stack = new StackPanel();
            stack.Children.Add(new TextBlock
            {
                Text = step.Degree,
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(AccentColor),
                HorizontalAlignment = HorizontalAlignment.Center
            });
            stack.Children.Add(new TextBlock
            {
                Text = step.Root + suffix,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(TextLight),
                HorizontalAlignment = HorizontalAlignment.Center
            });
            Grid.SetColumn(stack, 0);
            grid.Children.Add(stack);

            var deleteButton = new Button
            {
                Content = "×",
                Width = 18,
                Height = 18,
                FontSize = 10,
                Background = Brushes.Transparent,
                Foreground = new SolidColorBrush(DeleteColor),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(2, -2, -4, 0),
                Padding = new Thickness(0)
            };
            deleteButton.Click += (_, _) =>
            {
                if (capturedIndex < _progression.Count)
                {
                    _progression.RemoveAt(capturedIndex);
                    UpdateProgressionDisplay();
                }
            };
            Grid.SetColumn(deleteButton, 1);
            grid.Children.Add(deleteButton);

            border.Child = grid;
            ProgressionDisplay.Items.Add(border);
        }
    }

    private void Play_Click(object sender, RoutedEventArgs e)
    {
        if (_audio == null || _progression.Count == 0)
            return;

        int sampleRate = _audio.SampleRate;
        double beatDuration = 60.0 / _bpm;
        int samplesPerBeat = (int)(sampleRate * beatDuration);
        bool loop = LoopCheck.IsChecked == true;
        int repeats = loop ? 32 : 1;

        int oneCycleLength = samplesPerBeat * _progression.Count;
        float[] oneCycle = new float[oneCycleLength];

        for (int i = 0; i < _progression.Count; i++)
        {
            var step = _progression[i];
            var chord = ChordLibrary.Get(step.Root, step.ChordType);
            if (chord == null)
                continue;

            float[] chordSamples = ChordPlayer.Synthesize(
                chord,
                sampleRate,
                duration: (float)beatDuration * 0.95f,
                strumDelay: 0.02f);

            int offset = i * samplesPerBeat;
            int length = Math.Min(chordSamples.Length, oneCycleLength - offset);
            for (int j = 0; j < length; j++)
            {
                oneCycle[offset + j] += chordSamples[j];
            }
        }

        float[] buffer = new float[oneCycleLength * repeats];
        for (int repeat = 0; repeat < repeats; repeat++)
        {
            Array.Copy(oneCycle, 0, buffer, repeat * oneCycleLength, oneCycleLength);
        }

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
        if (BpmLabel != null)
        {
            BpmLabel.Text = $"{_bpm} BPM";
        }
    }

    private sealed class PresetListItem
    {
        public PresetListItem(ProgressionPreset preset)
        {
            Preset = preset;
        }

        public ProgressionPreset Preset { get; }

        public override string ToString() => Preset.Name;
    }

    private Button CreateButton(string content, double? width = null, double height = 26)
    {
        var button = new Button
        {
            Content = content,
            Height = height,
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Background = new SolidColorBrush(InactiveBg),
            BorderBrush = new SolidColorBrush(InactiveBg),
            BorderThickness = new Thickness(1),
            Foreground = new SolidColorBrush(TextLight),
            Cursor = System.Windows.Input.Cursors.Hand,
            Margin = new Thickness(0, 0, 6, 6),
            Padding = new Thickness(8, 0, 8, 0),
            Style = (Style)FindResource("ProgressionButton")
        };

        if (width.HasValue)
        {
            button.Width = width.Value;
        }

        return button;
    }
}
