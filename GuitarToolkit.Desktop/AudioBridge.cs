using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using GuitarToolkit.Core.Services;
using GuitarToolkit.UI;

namespace GuitarToolkit.Desktop;

public class AudioBridge : IAudioPlayback, IDisposable
{
    private WaveInEvent? _waveIn;
    private WaveOutEvent? _waveOut;
    private MixingSampleProvider? _mixer;

    private StoppableProvider? _currentPlayback;

    private static readonly WaveFormat Format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);

    public TunerEngine Tuner { get; }
    public MetronomeEngine Metronome { get; }

    public int SampleRate => 44100;

    public void PlaySamples(float[] samples)
    {
        // Останавливаем предыдущее
        _currentPlayback?.Stop();

        var provider = new StoppableProvider(samples, SampleRate);
        _currentPlayback = provider;
        _mixer?.AddMixerInput(provider);
    }

    public void StopPlayback()
    {
        _currentPlayback?.Stop();
        _currentPlayback = null;
    }

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
    /// Провайдер, который можно остановить извне.
    /// </summary>
    private class StoppableProvider : ISampleProvider
    {
        private readonly float[] _buffer;
        private int _pos;
        private bool _stopped;

        public WaveFormat WaveFormat { get; }

        public StoppableProvider(float[] buffer, int sampleRate)
        {
            _buffer = buffer;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1);
        }

        public void Stop() => _stopped = true;

        public int Read(float[] buffer, int offset, int count)
        {
            if (_stopped || _pos >= _buffer.Length)
            {
                Array.Clear(buffer, offset, count);
                return 0; // сигнал микшеру убрать этот источник
            }

            int toCopy = Math.Min(count, _buffer.Length - _pos);
            Array.Copy(_buffer, _pos, buffer, offset, toCopy);
            _pos += toCopy;

            if (toCopy < count)
                Array.Clear(buffer, offset + toCopy, count - toCopy);

            return toCopy;
        }
    }

    /// <summary>
    /// Постоянный поток метронома.
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
            Array.Clear(buffer, offset, count);
            float[] temp = new float[count];
            _engine.ProcessBlock(temp, count);
            Array.Copy(temp, 0, buffer, offset, count);
            return count;
        }
    }

    public AudioBridge()
    {
        Tuner = new TunerEngine(sampleRate: SampleRate);
        Metronome = new MetronomeEngine();
        Metronome.Initialize(SampleRate);

        _mixer = new MixingSampleProvider(Format) { ReadFully = true };
        _waveOut = new WaveOutEvent { DesiredLatency = 100 };
        _waveOut.Init(_mixer);
        _waveOut.Play();

        _mixer.AddMixerInput(new MetronomeProvider(Metronome, SampleRate));
    }

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
            AppLogger.Error("Audio input startup failed.", ex);
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

    public void Dispose()
    {
        StopInput();
        _waveOut?.Stop();
        _waveOut?.Dispose();
    }
}
