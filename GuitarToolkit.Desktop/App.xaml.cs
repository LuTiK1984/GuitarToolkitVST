using System.Windows;
using GuitarToolkit.Core.Services;
using GuitarToolkit.UI;

namespace GuitarToolkit.Desktop;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        ThemeManager.Apply(ThemeManager.Parse(UserSettings.Load().ThemeName));
        base.OnStartup(e);
    }
}
