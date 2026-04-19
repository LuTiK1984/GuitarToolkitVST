using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using GuitarToolkit.Core.Services;
using GuitarToolkit.UI;

namespace GuitarToolkit.Desktop;

/// <summary>
/// Мост между NAudio и движками Core.
/// Захватывает звук с выбранного устройства → подаёт в TunerEngine.
/// Воспроизводит метроном и аккорды через WaveOutEvent.
/// Реализует IAudioHost для UI.
/// </summary>
public class AudioBridge : IAudioPlayback, IDisposable
{
    private WaveInEvent? _waveIn;
    private WaveOutEvent? _waveOut;
    private MixingSampleProvider? _mixer;

    private float[]? _playbackBuffer;
    private int _playbackPos;

    private static readonly WaveFormat Format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);

    // ── Движки Core ──────────────────────────────────────────
    public TunerEngine Tuner { get; }
    public MetronomeEngine Metronome { get; }

    // ── IAudioHost ───────────────────────────────────────────
    public int SampleRate => 44100;

    public void PlaySamples(float[] samples)
    {
        // Воспроизводим через микшер NAudio
        float[] buf = new float[samples.Length];
        Array.Copy(samples, buf, samples.Length);

        byte[] bytes = new byte[buf.Length * 4];
        Buffer.BlockCopy(buf, 0, bytes, 0, bytes.Length);

        var provider = new RawSourceWaveStream(bytes, 0, bytes.Length, Format)
            .ToSampleProvider();
        _mixer?.AddMixerInput(provider);
    }

    // ── Устройства ввода ─────────────────────────────────────
    public static List<string> GetInputDevices()
    {
        var devices = new List<string>();
        for (int i = 0; i < WaveIn.DeviceCount; i++)
        {
            var caps = WaveIn.GetCapabilities(i);
            devices.Add(caps.ProductName);
        }
        return devices;
    }
    /// <summary>
    /// Постоянный аудиопоток метронома для микшера NAudio.
    /// </summary>
    private class MetronomeProvider : ISampleProvider
    {
        private readonly MetronomeEngine _engine;
        public WaveFormat WaveFormat { get; }

        public MetronomeProvider(MetronomeEngine engine, int sampleRate)
        {
            _engine = engine;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1);
        }

        public int Read(float[] buffer, int offset, int count)
        {
            // Очищаем буфер и заполняем метрономом
            Array.Clear(buffer, offset, count);
            float[] temp = new float[count];
            _engine.ProcessBlock(temp, count);
            Array.Copy(temp, 0, buffer, offset, count);
            return count;
        }
    }
    // ── Конструктор ──────────────────────────────────────────
    public AudioBridge()
    {
        Tuner = new TunerEngine(sampleRate: SampleRate);
        Metronome = new MetronomeEngine();
        Metronome.Initialize(SampleRate);

        // Выход — постоянно работающий микшер
        _mixer = new MixingSampleProvider(Format) { ReadFully = true };
        _waveOut = new WaveOutEvent { DesiredLatency = 100 };
        _waveOut.Init(_mixer);
        _waveOut.Play();
        // Метроном — постоянный поток в микшере
        _mixer.AddMixerInput(new MetronomeProvider(Metronome, SampleRate));
    }

    // ── Управление входом ────────────────────────────────────
    public void StartInput(int deviceIndex)
    {
        StopInput();

        try
        {
            _waveIn = new WaveInEvent
            {
                DeviceNumber = deviceIndex,
                WaveFormat = new WaveFormat(SampleRate, 1),
                BufferMilliseconds = 25
            };
            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.StartRecording();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Input error: {ex.Message}");
        }
    }

    public void StopInput()
    {
        _waveIn?.StopRecording();
        _waveIn?.Dispose();
        _waveIn = null;
    }

    private void OnDataAvailable(object? sender, WaveInEventArgs e)
    {
        int count = e.BytesRecorded / 2;
        float[] samples = new float[count];

        for (int i = 0; i < count; i++)
        {
            short s16 = BitConverter.ToInt16(e.Buffer, i * 2);
            samples[i] = s16 / 32768f;
        }

        Tuner.ProcessSamples(samples, count);
    }

    // ── Очистка ──────────────────────────────────────────────
    public void Dispose()
    {
        StopInput();
        _waveOut?.Stop();
        _waveOut?.Dispose();
    }
}
