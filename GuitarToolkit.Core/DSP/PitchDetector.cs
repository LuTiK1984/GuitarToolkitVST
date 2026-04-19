namespace GuitarToolkit.Core.DSP;

/// <summary>
/// Определение основной частоты методом FFT + Harmonic Product Spectrum.
/// Портировано из AudioService.Analyze() — алгоритм идентичен оригиналу.
/// </summary>
public class PitchDetector
{
    private readonly int _fftSize;
    private readonly int _sampleRate;

    public PitchDetector(int fftSize = 4096, int sampleRate = 44100)
    {
        _fftSize = fftSize;
        _sampleRate = sampleRate;
    }

    /// <summary>
    /// Анализирует массив сэмплов и возвращает частоту основного тона.
    /// Возвращает 0, если высоту определить не удалось.
    /// </summary>
    /// <param name="samples">Массив длиной не менее fftSize.</param>
    /// <param name="gain">Множитель усиления (применяется после окна Хэннинга).</param>
    public float DetectPitch(float[] samples, float gain = 1f)
    {
        if (samples.Length < _fftSize) return 0f;

        // Окно Хэннинга + усиление → комплексный буфер
        var buf = new ComplexFloat[_fftSize];
        for (int i = 0; i < _fftSize; i++)
        {
            double w = 0.5 * (1.0 - Math.Cos(2.0 * Math.PI * i / (_fftSize - 1)));
            buf[i] = new ComplexFloat(samples[i] * gain * (float)w, 0f);
        }

        Fft.Forward(buf);

        // Амплитудный спектр
        int half = _fftSize / 2;
        float[] spec = new float[half];
        for (int i = 0; i < half; i++)
            spec[i] = buf[i].Magnitude;

        // HPS — 4 гармоники
        float[] hps = (float[])spec.Clone();
        for (int h = 2; h <= 4; h++)
            for (int i = 0; i < half / h; i++)
                hps[i] *= spec[i * h];

        // Поиск пика в диапазоне гитары 70–1400 Гц
        int minBin = (int)(70f * _fftSize / _sampleRate);
        int maxBin = (int)(1400f * _fftSize / _sampleRate);
        int peak = minBin;
        float peakVal = 0f;

        for (int i = minBin; i < maxBin && i < hps.Length; i++)
        {
            if (hps[i] > peakVal) { peakVal = hps[i]; peak = i; }
        }

        if (peakVal < 1e-6f) return 0f;

        // Параболическая интерполяция для уточнения частоты
        float freq;
        if (peak > 0 && peak < spec.Length - 1)
        {
            float l = spec[peak - 1], c = spec[peak], r = spec[peak + 1];
            float d = 2f * c - l - r;
            float delta = Math.Abs(d) > 1e-10f ? 0.5f * (r - l) / d : 0f;
            freq = (peak + delta) * _sampleRate / (float)_fftSize;
        }
        else
        {
            freq = peak * _sampleRate / (float)_fftSize;
        }

        return (freq >= 70f && freq <= 1400f) ? freq : 0f;
    }
}
