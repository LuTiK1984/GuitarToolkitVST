using System.Windows.Controls;
using GuitarToolkit.Core.Services;

namespace GuitarToolkit.UI;

public partial class ToolkitHostView : UserControl
{
    public ToolkitHostView(TunerEngine tuner, MetronomeEngine metronome, IAudioPlayback audioHost)
    {
        InitializeComponent();

        TunerTab.Initialize(tuner);
        MetronomeTab.Initialize(metronome);
        ChordTab.Initialize(audioHost);
    }
}
