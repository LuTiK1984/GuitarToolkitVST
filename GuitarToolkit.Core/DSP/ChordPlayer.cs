using GuitarToolkit.Core.Models;

namespace GuitarToolkit.Core.DSP;

/// <summary>
/// Синтезатор звучания аккордов с плавным затуханием.
/// </summary>
public static class ChordPlayer
{
    private static readonly float[] OpenStringFreqs =
    {
        82.41f, 110.00f, 146.83f, 196.00f, 246.94f, 329.63f
    };

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
            if (fret < 0) continue;

            float freq = OpenStringFreqs[s] * MathF.Pow(2f, fret / 12f);
            int offset = (int)(s * strumDelay * sampleRate);

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

        // Плавный fade-out на последних 500 сэмплах
        int fadeLen = Math.Min(500, totalSamples);
        for (int i = 0; i < fadeLen; i++)
        {
            float fade = (float)i / fadeLen;
            buffer[totalSamples - fadeLen + i] *= fade < 0.5f ? 1f : 1f - (fade - 0.5f) * 2f;
        }

        return buffer;
    }
}
