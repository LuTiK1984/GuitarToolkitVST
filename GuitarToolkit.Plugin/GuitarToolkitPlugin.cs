using System.Windows.Controls;
using AudioPlugSharp;
using AudioPlugSharpWPF;
using GuitarToolkit.Core.Services;
using GuitarToolkit.UI;

namespace GuitarToolkit.Plugin;

/// <summary>
/// VST3-плагин GuitarToolkit. Реализует IAudioPlayback для передачи звука из UI.
/// UI полностью делегирован в GuitarToolkit.UI (ToolkitHostView).
/// </summary>
public class GuitarToolkitPlugin : AudioPluginWPF, IAudioPlayback
{
    private DoubleAudioIOPort _monoInput = null!;
    private DoubleAudioIOPort _stereoOutput = null!;

    public TunerEngine Tuner { get; private set; } = null!;
    public MetronomeEngine Metronome { get; private set; } = null!;

    // Воспроизведение (аккорды и т.п.)
    private float[]? _playbackBuffer;
    private int _playbackPos;

    // ── IAudioPlayback ───────────────────────────────────────────
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

    // ── VST3 ─────────────────────────────────────────────────
    public GuitarToolkitPlugin()
    {
        Company = "BSTU";
        Website = "";
        Contact = "";
        PluginName = "GuitarToolkit";
        PluginCategory = "Fx";
        PluginVersion = "1.0.0";
        PluginID = 0x47546B7401000001;
        HasUserInterface = true;
        EditorWidth = 800;
        EditorHeight = 550;
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
        Span<double> input = _monoInput.GetAudioBuffer(0);
        Span<double> outL = _stereoOutput.GetAudioBuffer(0);
        Span<double> outR = _stereoOutput.GetAudioBuffer(1);
        int len = input.Length;

        // Проброс входа
        for (int i = 0; i < len; i++)
        {
            outL[i] = input[i];
            outR[i] = input[i];
        }

        // Тюнер
        try
        {
            float[] floatBuf = new float[len];
            for (int i = 0; i < len; i++)
                floatBuf[i] = (float)input[i];
            Tuner.ProcessSamples(floatBuf, len);
        }
        catch { }

        // Метроном
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

        // Воспроизведение (аккорды)
        try
        {
            if (_playbackBuffer != null && _playbackPos < _playbackBuffer.Length)
            {
                for (int i = 0; i < len && _playbackPos < _playbackBuffer.Length; i++, _playbackPos++)
                {
                    outL[i] += _playbackBuffer[_playbackPos];
                    outR[i] += _playbackBuffer[_playbackPos];
                }
            }
        }
        catch { }
    }

    public override UserControl GetEditorView()
    {
        return new ToolkitHostView(Tuner, Metronome, this);
    }
}
