using System.Windows.Controls;
using System.Threading;
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

    private float[] _inputFloatBuffer = Array.Empty<float>();
    private float[] _metronomeBuffer = Array.Empty<float>();

    private static void EnsureBuffer(ref float[] buffer, int length)
    {
        if (buffer.Length < length)
            buffer = new float[length];
    }


    public int SampleRate
    {
        get
        {
            try { return (int)Host.SampleRate; } catch { return 44100; }
        }
    }

    public void PlaySamples(float[] samples)
    {
        _playbackPos = 0;
        Volatile.Write(ref _playbackBuffer, samples);
    }

    public void StopPlayback()
    {
        Volatile.Write(ref _playbackBuffer, null);
        _playbackPos = 0;
    }

    public GuitarToolkitPlugin()
    {
        Company = "Samizdat";
        Website = "";
        Contact = "";
        PluginName = "GuitarToolkit";
        PluginCategory = "Fx";
        PluginVersion = "1.0.0";
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
        Span<double> input = _monoInput.GetAudioBuffer(0);
        Span<double> outL = _stereoOutput.GetAudioBuffer(0);
        Span<double> outR = _stereoOutput.GetAudioBuffer(1);
        int len = input.Length;

        EnsureBuffer(ref _inputFloatBuffer, len);
        EnsureBuffer(ref _metronomeBuffer, len);

        for (int i = 0; i < len; i++)
        {
            double sample = input[i];
            outL[i] = sample;
            outR[i] = sample;
            _inputFloatBuffer[i] = (float)sample;
        }

        try
        {
            Tuner.ProcessSamples(_inputFloatBuffer, len);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Tuner processing error: {ex.Message}");
        }

        try
        {
            Array.Clear(_metronomeBuffer, 0, len);
            Metronome.ProcessBlock(_metronomeBuffer, len);

            for (int i = 0; i < len; i++)
            {
                double click = _metronomeBuffer[i];
                outL[i] += click;
                outR[i] += click;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Metronome processing error: {ex.Message}");
        }

        try
        {
            var buf = Volatile.Read(ref _playbackBuffer);
            if (buf != null && _playbackPos < buf.Length)
            {
                for (int i = 0; i < len && _playbackPos < buf.Length; i++, _playbackPos++)
                {
                    double sample = buf[_playbackPos];
                    outL[i] += sample;
                    outR[i] += sample;
                }

                if (_playbackPos >= buf.Length)
                {
                    Volatile.Write(ref _playbackBuffer, null);
                    _playbackPos = 0;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Playback processing error: {ex.Message}");
        }
    }


    public override UserControl GetEditorView()
    {
        return new ToolkitHostView(Tuner, Metronome, this);
    }
}
