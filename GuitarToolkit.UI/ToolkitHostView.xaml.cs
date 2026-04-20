using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GuitarToolkit.Core.Services;

namespace GuitarToolkit.UI;

public partial class ToolkitHostView : UserControl
{
    private readonly UserSettings _settings;

    public ToolkitHostView(TunerEngine tuner, MetronomeEngine metronome, IAudioPlayback audioHost)
    {
        InitializeComponent();

        _settings = UserSettings.Load();

        TunerTab.Initialize(tuner, _settings);
        MetronomeTab.Initialize(metronome, _settings);
        ChordTab.Initialize(audioHost, _settings);
        IntervalTab.Initialize(audioHost);
        ProgressionTab.Initialize(audioHost, _settings);
        CircleTab.Initialize(audioHost);
        FretboardTab.Initialize(audioHost, _settings);

        // Горячие клавиши
        Focusable = true;
        Loaded += (s, e) => Focus();
        PreviewKeyDown += OnKeyDown;
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
        {
            // Пробел — старт/стоп метронома (с любой вкладки)
            MetronomeTab.ToggleStartStop();
            e.Handled = true;
        }
    }

    /// <summary>
    /// Вызывается при закрытии — сохраняет настройки.
    /// </summary>
    public void SaveSettings()
    {
        TunerTab.SaveTo(_settings);
        MetronomeTab.SaveTo(_settings);
        ChordTab.SaveTo(_settings);
        ProgressionTab.SaveTo(_settings);
        FretboardTab.SaveTo(_settings);
        _settings.Save();
    }
}
