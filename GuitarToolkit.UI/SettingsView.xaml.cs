using System.Windows;
using System.Windows.Controls;
using GuitarToolkit.Core.Services;

namespace GuitarToolkit.UI;

public partial class SettingsView : UserControl
{
    private UserSettings? _settings;
    private bool _isUpdating;

    public SettingsView()
    {
        InitializeComponent();
    }

    public event EventHandler<ToolkitTheme>? ThemeChanged;

    public void Initialize(UserSettings settings)
    {
        _settings = settings;
        _isUpdating = true;
        try
        {
            LanguageBox.SelectedIndex = settings.Language == "en-US" ? 1 : 0;
            UpdateThemeButtons(ThemeManager.Parse(settings.ThemeName));
        }
        finally
        {
            _isUpdating = false;
        }
    }

    private void DarkTheme_Click(object sender, RoutedEventArgs e) => SetTheme(ToolkitTheme.Dark);

    private void LightTheme_Click(object sender, RoutedEventArgs e) => SetTheme(ToolkitTheme.Light);

    private void SetTheme(ToolkitTheme theme)
    {
        if (_settings != null)
        {
            _settings.ThemeName = ThemeManager.ToSettingsValue(theme);
            _settings.Save();
        }

        UpdateThemeButtons(theme);
        ThemeChanged?.Invoke(this, theme);
    }

    private void UpdateThemeButtons(ToolkitTheme theme)
    {
        if (DarkThemeButton == null || LightThemeButton == null) return;

        DarkThemeButton.Style = (Style)FindResource(theme == ToolkitTheme.Dark ? "SelectedButton" : "SettingsButton");
        LightThemeButton.Style = (Style)FindResource(theme == ToolkitTheme.Light ? "SelectedButton" : "SettingsButton");
    }

    private void LanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isUpdating || _settings == null) return;
        _settings.Language = LanguageBox.SelectedIndex == 1 ? "en-US" : "ru-RU";
    }
}
