namespace GuitarToolkit.Core.DSP;

/// <summary>
/// Синтез отдельных нот для тренажёра интервалов.
/// </summary>
public static class NoteSynth
{
    /// <summary>
    /// Синтезирует одну ноту (синусоида + 2-я гармоника + затухание).
    /// </summary>
    public static float[] GenerateNote(float frequency, int sampleRate = 44100,
        float duration = 0.8f, float volume = 0.3f)
    {
        int count = (int)(sampleRate * duration);
        float[] buf = new float[count];

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / sampleRate;
            float env = MathF.Exp(-t * 3f);
            buf[i] = (MathF.Sin(2f * MathF.PI * frequency * t) * 0.8f
                     + MathF.Sin(2f * MathF.PI * frequency * 2f * t) * 0.2f)
                     * env * volume;
        }

        return buf;
    }

    /// <summary>
    /// Синтезирует два звука последовательно (для интервалов).
    /// </summary>
    public static float[] GenerateInterval(float freq1, float freq2,
        int sampleRate = 44100, float noteDuration = 0.8f, float gap = 0.3f)
    {
        float[] note1 = GenerateNote(freq1, sampleRate, noteDuration);
        float[] note2 = GenerateNote(freq2, sampleRate, noteDuration);
        int gapSamples = (int)(gap * sampleRate);

        float[] result = new float[note1.Length + gapSamples + note2.Length];
        Array.Copy(note1, 0, result, 0, note1.Length);
        Array.Copy(note2, 0, result, note1.Length + gapSamples, note2.Length);

        return result;
    }
}
