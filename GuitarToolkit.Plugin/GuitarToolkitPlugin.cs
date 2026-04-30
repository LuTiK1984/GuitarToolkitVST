using System.Windows.Controls;
using AudioPlugSharp;
using AudioPlugSharpWPF;
using GuitarToolkit.Core.Services;
using GuitarToolkit.UI;

namespace GuitarToolkit.Plugin;

public class GuitarToolkitPlugin : AudioPluginWPF, IAudioPlayback
{
    private DoubleAudioIOPort _monoInput = null!;
    private DoubleAudioIOPort _stereoOutput = null!;

    public TunerEngine Tuner { get; private set; } = null!;
    public MetronomeEngine Metronome { get; private set; } = null!;

    private float[]? _playbackBuffer;
    private int _playbackPos;


    public int SampleRate
    {
        get
        {
            try { return (int)Host.SampleRate; } catch { return 44100; }
        }
    }

    public void PlaySamples(float[] samples)
    {
        _playbackBuffer = samples;
        _playbackPos = 0;
    }

    public void StopPlayback()
    {
        _playbackBuffer = null;
        _playbackPos = 0;
    }

    public GuitarToolkitPlugin()
    {
        Company = "Samizdat";
        Website = "";
        Contact = "";
        PluginName = "GuitarToolkit";
        PluginCategory = "Fx";
        PluginVersion = "1.3.2";
        PluginID = 0x47546B7401000001;
        HasUserInterface = true;
        EditorWidth = 950;
        EditorHeight = 650;
    }

    public override void Initialize()
    {
        base.Initialize();

        InputPorts = new AudioIOPort[]
        {
            _monoInput = new DoubleAudioIOPort("Mono Input", EAudioChannelConfiguration.Mono)
        };
        OutputPorts = new AudioIOPort[]
        {
            _stereoOutput = new DoubleAudioIOPort("Stereo Output", EAudioChannelConfiguration.Stereo)
        };

        int sr = 44100;
        try { sr = (int)Host.SampleRate; } catch { }
        if (sr <= 0) sr = 44100;

        Tuner = new TunerEngine(sampleRate: sr);
        Metronome = new MetronomeEngine();
        Metronome.Initialize(sr);
    }

    public override void Process()
    {
        try
        {
            Span<double> input = _monoInput.GetAudioBuffer(0);
            Span<double> outL = _stereoOutput.GetAudioBuffer(0);
            Span<double> outR = _stereoOutput.GetAudioBuffer(1);
            int len = input.Length;

            for (int i = 0; i < len; i++)
            {
                outL[i] = input[i];
                outR[i] = input[i];
            }

            try
            {
                float[] floatBuf = new float[len];
                for (int i = 0; i < len; i++)
                    floatBuf[i] = (float)input[i];
                Tuner.ProcessSamples(floatBuf, len);
            }
            catch { }

            try
            {
                float[] metroBuf = new float[len];
                Metronome.ProcessBlock(metroBuf, len);
                for (int i = 0; i < len; i++)
                {
                    outL[i] += metroBuf[i];
                    outR[i] += metroBuf[i];
                }
            }
            catch { }

            var buf = _playbackBuffer;
            if (buf != null && _playbackPos < buf.Length)
            {
                for (int i = 0; i < len && _playbackPos < buf.Length; i++, _playbackPos++)
                {
                    outL[i] += buf[_playbackPos];
                    outR[i] += buf[_playbackPos];
                }

                if (_playbackPos >= buf.Length)
                {
                    _playbackBuffer = null;
                    _playbackPos = 0;
                }
            }
        }
        catch
        {
        }
    }


    public override UserControl GetEditorView()
    {
        return new ToolkitHostView(Tuner, Metronome, this, enableTabs: true);
    }
}
