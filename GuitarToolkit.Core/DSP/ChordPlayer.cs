using GuitarToolkit.Core.Models;

namespace GuitarToolkit.Core.DSP;

/// <summary>
/// Простой синтезатор звучания аккорда: каждая струна — затухающая синусоида,
/// со сдвигом по времени (эффект удара по струнам).
/// </summary>
public static class ChordPlayer
{
    // Частоты открытых струн стандартного строя (6→1: E2 A2 D3 G3 B3 E4)
    private static readonly float[] OpenStringFreqs =
    {
        82.41f,   // E2
        110.00f,  // A2
        146.83f,  // D3
        196.00f,  // G3
        246.94f,  // B3
        329.63f   // E4
    };

    /// <summary>
    /// Синтезирует звучание аккорда.
    /// </summary>
    /// <param name="chord">Определение аккорда.</param>
    /// <param name="sampleRate">Частота дискретизации.</param>
    /// <param name="duration">Длительность звучания (секунды).</param>
    /// <param name="strumDelay">Задержка между струнами (секунды, эффект удара).</param>
    /// <returns>Массив сэмплов.</returns>
    public static float[] Synthesize(
        ChordDefinition chord,
        int sampleRate = 44100,
        float duration = 2.0f,
        float strumDelay = 0.025f)
    {
        int totalSamples = (int)(duration * sampleRate);
        float[] buffer = new float[totalSamples];

        for (int s = 0; s < 6; s++)
        {
            int fret = chord.Frets[s];
            if (fret < 0) continue; // заглушенная струна

            // Частота = открытая струна × 2^(лад/12)
            float freq = OpenStringFreqs[s] * MathF.Pow(2f, fret / 12f);

            // Смещение для эффекта удара (от 6-й к 1-й струне)
            int offset = (int)(s * strumDelay * sampleRate);

            // Генерация: синусоида + вторая гармоника + экспоненциальное затухание
            int count = totalSamples - offset;
            for (int i = 0; i < count; i++)
            {
                float t = (float)i / sampleRate;
                float envelope = MathF.Exp(-t * 2.5f);

                float sample = MathF.Sin(2f * MathF.PI * freq * t) * 0.7f
                             + MathF.Sin(2f * MathF.PI * freq * 2f * t) * 0.2f
                             + MathF.Sin(2f * MathF.PI * freq * 3f * t) * 0.1f;

                buffer[i + offset] += sample * envelope * 0.12f;
            }
        }

        return buffer;
    }
}
