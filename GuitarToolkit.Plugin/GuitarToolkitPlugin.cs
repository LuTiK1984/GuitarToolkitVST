using System.Windows.Controls;
using AudioPlugSharp;
using AudioPlugSharpWPF;
using GuitarToolkit.Core.Services;
using GuitarToolkit.Plugin.UI;

namespace GuitarToolkit.Plugin;

/// <summary>
/// Главный класс VST3-плагина GuitarToolkit.
/// Наследует AudioPluginWPF — это даёт VST3-совместимость + WPF GUI.
/// </summary>
public class GuitarToolkitPlugin : AudioPluginWPF
{
    private DoubleAudioIOPort _monoInput = null!;
    private DoubleAudioIOPort _stereoOutput = null!;

    // Движки Core — доступны из UI через свойства
    public TunerEngine Tuner { get; private set; } = null!;
    public MetronomeEngine Metronome { get; private set; } = null!;

    public GuitarToolkitPlugin()
    {
        Company = "BSTU";
        Website = "";
        Contact = "";
        PluginName = "GuitarToolkit";
        PluginCategory = "Fx";
        PluginVersion = "1.0.0";

        // Уникальный 64-битный ID плагина (не менять после первого релиза!)
        PluginID = 0x47546B7401000001;

        HasUserInterface = true;
        EditorWidth = 800;
        EditorHeight = 550;
    }

    public override void Initialize()
    {
        base.Initialize();

        // Аудио-порты: моно вход (гитара), стерео выход
        InputPorts = new AudioIOPort[]
        {
            _monoInput = new DoubleAudioIOPort("Mono Input", EAudioChannelConfiguration.Mono)
        };
        OutputPorts = new AudioIOPort[]
        {
            _stereoOutput = new DoubleAudioIOPort("Stereo Output", EAudioChannelConfiguration.Stereo)
        };

        // Инициализация движков с sample rate от хоста
        int sr = (int)Host.SampleRate;
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

        // 1. Всегда пишем выход (проброс), даже если дальше что-то упадёт
        for (int i = 0; i < len; i++)
        {
            outL[i] = input[i];
            outR[i] = input[i];
        }

        // 2. Тюнер — отдельно
        try
        {
            float[] floatBuf = new float[len];
            for (int i = 0; i < len; i++)
                floatBuf[i] = (float)input[i];
            Tuner.ProcessSamples(floatBuf, len);
        }
        catch { }

        // 3. Метроном — отдельно, микшируем поверх
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
    }

    /// <summary>
    /// Создаёт WPF-интерфейс плагина (вызывается хостом при открытии GUI).
    /// </summary>
    public override UserControl GetEditorView()
    {
        return new PluginView(this);
    }
}
