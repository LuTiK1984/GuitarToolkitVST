namespace GuitarToolkit.Core.DSP;

/// <summary>
/// Генерация коротких тональных щелчков для метронома.
/// </summary>
public static class ClickGenerator
{
    /// <summary>
    /// Генерирует щелчок: синусоида с линейным затуханием.
    /// </summary>
    public static float[] Generate(float frequency, int durationMs, int sampleRate = 44100)
    {
        int count = sampleRate * durationMs / 1000;
        float[] buffer = new float[count];

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = 1f - (float)i / count;
            buffer[i] = MathF.Sin(2f * MathF.PI * frequency * t) * envelope;
        }

        return buffer;
    }
}
