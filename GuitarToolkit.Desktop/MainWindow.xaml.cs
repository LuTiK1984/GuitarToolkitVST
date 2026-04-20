using System.Windows;
using System.Windows.Controls;
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
            ContentArea.Content = _toolkitView;

            var devices = AudioBridge.GetInputDevices();
            if (devices.Count > 0)
            {
                foreach (var d in devices)
                    DeviceBox.Items.Add(d);
                DeviceBox.SelectedIndex = Math.Clamp(settings.LastInputDevice, 0, devices.Count - 1);
            }
            else
            {
                StatusLabel.Text = "Устройства ввода не найдены";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Ошибка запуска");
            _audio = new AudioBridge();
        }
    }

    private void DeviceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DeviceBox.SelectedIndex >= 0)
        {
            _audio.StartInput(DeviceBox.SelectedIndex);
            StatusLabel.Text = "● Запись";
        }
    }

    private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        // Сохраняем настройки
        _toolkitView?.SaveSettings();

        // Сохраняем выбранное устройство отдельно
        var settings = UserSettings.Load();
        settings.LastInputDevice = DeviceBox.SelectedIndex;
        settings.Save();

        _audio.Dispose();
    }
}
