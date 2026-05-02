using System.Windows;
using GuitarToolkit.Core.Services;
using GuitarToolkit.UI;

namespace GuitarToolkit.Desktop;

public partial class MainWindow : Window
{
    private readonly AudioBridge _audio;
    private ToolkitHostView? _toolkitView;

    public MainWindow()
    {
        InitializeComponent();

        try
        {
            _audio = new AudioBridge();

            var settings = UserSettings.Load();
            _toolkitView = new ToolkitHostView(_audio.Tuner, _audio.Metronome, _audio);
            _toolkitView.InputDeviceSelected += ToolkitView_InputDeviceSelected;
            ContentArea.Content = _toolkitView;

            var devices = AudioBridge.GetInputDevices();
            if (devices.Count > 0)
            {
                int selectedIndex = Math.Clamp(settings.LastInputDevice, 0, devices.Count - 1);
                _toolkitView.SetInputDevices(devices, selectedIndex);
                StartInput(selectedIndex);
            }
            else
            {
                _toolkitView.SetInputDevices(Array.Empty<string>(), -1);
                _toolkitView.SetInputStatus("\u0423\u0441\u0442\u0440\u043E\u0439\u0441\u0442\u0432\u0430 \u043D\u0435 \u043D\u0430\u0439\u0434\u0435\u043D\u044B", false);
            }
        }
        catch (Exception ex)
        {
            AppLogger.Error("Desktop startup failed.", ex);
            MessageBox.Show(ex.ToString(), "\u041E\u0448\u0438\u0431\u043A\u0430 \u0437\u0430\u043F\u0443\u0441\u043A\u0430");
            _audio = new AudioBridge();
        }
    }

    private void ToolkitView_InputDeviceSelected(object? sender, int deviceIndex)
    {
        StartInput(deviceIndex);
    }

    private void StartInput(int deviceIndex)
    {
        if (deviceIndex < 0) return;

        _audio.StartInput(deviceIndex);
        _toolkitView?.SetInputStatus("\u25CF \u0417\u0430\u043F\u0438\u0441\u044C", true);
    }

    private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _toolkitView?.SaveSettings();

        var settings = UserSettings.Load();
        int selectedInputDeviceIndex = _toolkitView?.SelectedInputDeviceIndex ?? -1;
        if (selectedInputDeviceIndex >= 0)
            settings.LastInputDevice = selectedInputDeviceIndex;
        settings.Save();

        _audio.Dispose();
    }
}
