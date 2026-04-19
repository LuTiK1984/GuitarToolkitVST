using System.Windows.Controls;

namespace GuitarToolkit.Plugin.UI;

public partial class PluginView : UserControl
{
    public PluginView(GuitarToolkitPlugin plugin)
    {
        InitializeComponent();

        TunerTab.Initialize(plugin.Tuner);
        MetronomeTab.Initialize(plugin.Metronome);
    }
}
