using System.Windows;
using System.Windows.Controls;
using GuitarToolkit.UI;

namespace GuitarToolkit.Desktop;

public partial class MainWindow : Window
{
    private readonly AudioBridge _audio;

    public MainWindow()
    {
        InitializeComponent();

        _audio = new AudioBridge();

        // Создаём общий UI (те же вкладки что в плагине)
        var view = new ToolkitHostView(_audio.Tuner, _audio.Metronome, _audio);
        ContentArea.Content = view;

        // Заполняем список устройств ввода
        var devices = AudioBridge.GetInputDevices();
        if (devices.Count > 0)
        {
            foreach (var d in devices)
                DeviceBox.Items.Add(d);
            DeviceBox.SelectedIndex = 0;
        }
        else
        {
            StatusLabel.Text = "Устройства ввода не найдены";
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
        _audio.Dispose();
    }
}
