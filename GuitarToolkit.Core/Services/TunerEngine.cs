using GuitarToolkit.Core.DSP;
using GuitarToolkit.Core.Models;

namespace GuitarToolkit.Core.Services;

/// <summary>
/// Движок тюнера: принимает сырые сэмплы, определяет ноту, частоту, отклонение.
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

    // ── Настройки ────────────────────────────────────────────
    public float Gain { get; set; } = 1f;
    public float SilenceThreshold { get; set; } = 0.005f;
    public float ReferenceA { get; set; } = 440f;
    public int StableThreshold { get; set; } = 3;  // было 5 — быстрее реакция

    // ── Текущее состояние ────────────────────────────────────
    public string CurrentNote { get; private set; } = "—";
    public float CurrentFrequency { get; private set; }
    public float CurrentCents { get; private set; }
    public float CurrentVolume { get; private set; }

    // ── События ──────────────────────────────────────────────
    public event Action<string, float, float>? NoteDetected;
    public event Action<float>? VolumeChanged;

    public TunerEngine(int fftSize = 8192, int sampleRate = 44100)
    {
        _fftSize = fftSize;
        _sampleRate = sampleRate;
        _ring = new float[fftSize];
        _detector = new PitchDetector(fftSize, sampleRate);
    }

    public void ProcessSamples(float[] samples, int count)
    {
        // Заполняем кольцевой буфер с усилением
        for (int i = 0; i < count; i++)
        {
            _ring[_ringPos & (_fftSize - 1)] = samples[i] * Gain;
            _ringPos++;
        }
        _filled = Math.Min(_filled + count, _fftSize);

        // RMS-громкость
        float sum = 0f;
        for (int i = 0; i < _fftSize; i++) sum += _ring[i] * _ring[i];
        CurrentVolume = MathF.Sqrt(sum / _fftSize);
        VolumeChanged?.Invoke(CurrentVolume);

        // Порог тишины
        if (_filled < _fftSize) return;
        if (CurrentVolume < SilenceThreshold)
        {
            _stableCount = 0;
            _smoothedFreq = 0f;
            CurrentNote = "—";
            return;
        }

        // Копируем буфер по порядку
        float[] ordered = new float[_fftSize];
        int start = _ringPos % _fftSize;
        for (int i = 0; i < _fftSize; i++)
            ordered[i] = _ring[(start + i) % _fftSize];

        float freq = _detector.DetectPitch(ordered);
        if (freq <= 0f) return;

        // Лёгкое сглаживание: 30% старое, 70% новое (было 50/50)
        // Если частота прыгнула больше чем на полтона — сбрасываем, берём новую
        if (_smoothedFreq < 1f || MathF.Abs(freq - _smoothedFreq) / _smoothedFreq > 0.05f)
        {
            _smoothedFreq = freq;
        }
        else
        {
            _smoothedFreq = _smoothedFreq * 0.3f + freq * 0.7f;
        }

        var (note, cents) = NoteUtils.FrequencyToNote(_smoothedFreq, ReferenceA);

        // Стабилизация
        if (note == _lastNote)
        {
            _stableCount++;
        }
        else
        {
            _stableCount = 0;
            _lastNote = note;
        }

        // Обновляем всегда (для плавности стрелки), но ноту показываем после стабилизации
        CurrentFrequency = _smoothedFreq;
        CurrentCents = cents;

        if (_stableCount >= StableThreshold)
        {
            CurrentNote = note;
        }

        NoteDetected?.Invoke(CurrentNote, _smoothedFreq, cents);
    }

    public void Reset()
    {
        Array.Clear(_ring);
        _ringPos = 0;
        _filled = 0;
        _smoothedFreq = 0f;
        _stableCount = 0;
        _lastNote = "—";
        CurrentNote = "—";
        CurrentFrequency = 0f;
        CurrentCents = 0f;
        CurrentVolume = 0f;
    }
}
