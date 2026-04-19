using GuitarToolkit.Core.DSP;
using GuitarToolkit.Core.Models;

namespace GuitarToolkit.Core.Services;

/// <summary>
/// Движок тюнера: принимает сырые сэмплы, определяет ноту, частоту, отклонение.
/// Не зависит от источника звука — работает и с NAudio, и с VST-шиной.
/// </summary>
public class TunerEngine
{
    private readonly PitchDetector _detector;
    private readonly float[] _ring;
    private readonly int _fftSize;
    private readonly int _sampleRate;
    private int _ringPos;
    private int _filled;

    // Сглаживание и стабилизация
    private float _smoothedFreq;
    private string _lastNote = "—";
    private int _stableCount;
    private int _newSamples;
    private const int AnalysisInterval = 1024; // анализ каждые ~23 мс при 44100

    // ── Настройки ────────────────────────────────────────────
    public float Gain { get; set; } = 1f;
    public float SilenceThreshold { get; set; } = 0.00005f;
    public float ReferenceA { get; set; } = 440f;
    public int StableThreshold { get; set; } = 3;

    // ── Текущее состояние (читается из UI) ───────────────────
    public string CurrentNote { get; private set; } = "—";
    public float CurrentFrequency { get; private set; }
    public float CurrentCents { get; private set; }
    public float CurrentVolume { get; private set; }

    // ── События ──────────────────────────────────────────────
    public event Action<string, float, float>? NoteDetected;
    public event Action<float>? VolumeChanged;

    public TunerEngine(int fftSize = 4096, int sampleRate = 44100)
    {
        _fftSize = fftSize;
        _sampleRate = sampleRate;
        _ring = new float[fftSize];
        _detector = new PitchDetector(fftSize, sampleRate);
    }

    /// <summary>
    /// Подать очередную порцию сэмплов (вызывается из Process() или DataAvailable).
    /// </summary>
    public void ProcessSamples(float[] samples, int count)
    {
        // Заполняем кольцевой буфер
        for (int i = 0; i < count; i++)
        {
            _ring[_ringPos & (_fftSize - 1)] = samples[i];
            _ringPos++;
        }
        _filled = Math.Min(_filled + count, _fftSize);
        _newSamples += count;

        // RMS-громкость — считаем всегда
        float sum = 0f;
        for (int i = 0; i < _fftSize; i++) sum += _ring[i] * _ring[i];
        CurrentVolume = MathF.Sqrt(sum / _fftSize);
        VolumeChanged?.Invoke(CurrentVolume);

        // Анализируем не чаще, чем раз в AnalysisInterval сэмплов
        //if (_newSamples < AnalysisInterval) return;
        _newSamples = 0;

        if (_filled < _fftSize) return;
        if (CurrentVolume < SilenceThreshold)
        {
            _stableCount = 0;
            _smoothedFreq = 0f;
            return;
        }

        // Копируем кольцевой буфер по порядку
        float[] ordered = new float[_fftSize];
        int start = _ringPos % _fftSize;
        for (int i = 0; i < _fftSize; i++)
            ordered[i] = _ring[(start + i) % _fftSize];

        float freq = _detector.DetectPitch(ordered, Gain);
        if (freq <= 0f) return;

        // Сглаживание
        _smoothedFreq = _smoothedFreq < 1f
            ? freq
            : _smoothedFreq * 0.5f + freq * 0.5f;

        var (note, cents) = NoteUtils.FrequencyToNote(_smoothedFreq, ReferenceA);

        // Стабилизация — нота должна повториться StableThreshold раз
        if (note == _lastNote)
        {
            _stableCount++;
        }
        else
        {
            _stableCount = 0;
            _lastNote = note;
            _smoothedFreq = freq;
            (note, cents) = NoteUtils.FrequencyToNote(_smoothedFreq, ReferenceA);
        }

        CurrentNote = note;
        CurrentFrequency = _smoothedFreq;
        CurrentCents = cents;
        NoteDetected?.Invoke(note, _smoothedFreq, cents);
    }

    /// <summary>
    /// Сброс состояния (при смене строя, например).
    /// </summary>
    public void Reset()
    {
        Array.Clear(_ring);
        _ringPos = 0;
        _filled = 0;
        _newSamples = 0;
        _smoothedFreq = 0f;
        _stableCount = 0;
        _lastNote = "—";
        CurrentNote = "—";
        CurrentFrequency = 0f;
        CurrentCents = 0f;
        CurrentVolume = 0f;
    }
}
