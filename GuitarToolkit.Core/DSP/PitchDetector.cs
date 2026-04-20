namespace GuitarToolkit.Core.DSP;

/// <summary>
/// Определение основной частоты методом FFT + Harmonic Product Spectrum.
/// Улучшения: интерполяция по HPS, проверка качества пика, адаптивный порог.
/// </summary>
public class PitchDetector
{
    private readonly int _fftSize;
    private readonly int _sampleRate;

    public PitchDetector(int fftSize = 8192, int sampleRate = 44100)
    {
        _fftSize = fftSize;
        _sampleRate = sampleRate;
    }

    public float DetectPitch(float[] samples, float gain = 1f)
    {
        if (samples.Length < _fftSize) return 0f;

        // Окно Хэннинга → комплексный буфер
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

        // HPS — 5 гармоник (было 4)
        float[] hps = (float[])spec.Clone();
        for (int h = 2; h <= 5; h++)
            for (int i = 0; i < half / h; i++)
                hps[i] *= spec[i * h];

        // Диапазон гитары: 70–1400 Гц
        int minBin = (int)(70f * _fftSize / _sampleRate);
        int maxBin = Math.Min((int)(1400f * _fftSize / _sampleRate), half / 5);
        int peak = minBin;
        float peakVal = 0f;

        for (int i = minBin; i < maxBin && i < hps.Length; i++)
        {
            if (hps[i] > peakVal) { peakVal = hps[i]; peak = i; }
        }

        // Проверка качества: пик должен быть значительно выше среднего
        float avg = 0f;
        int count = 0;
        for (int i = minBin; i < maxBin && i < hps.Length; i++)
        {
            avg += hps[i];
            count++;
        }
        avg /= Math.Max(count, 1);

        if (peakVal < avg * 5f) return 0f;  // пик слабый — шум

        // Параболическая интерполяция ПО HPS (не по spec!)
        float freq;
        if (peak > 0 && peak < hps.Length - 1)
        {
            float l = hps[peak - 1], c = hps[peak], r = hps[peak + 1];
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
