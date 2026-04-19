namespace GuitarToolkit.Core.Models;

/// <summary>
/// Утилиты для работы с нотами: преобразование частота ↔ нота.
/// </summary>
public static class NoteUtils
{
    private static readonly string[] NoteNames =
        { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    private static readonly Dictionary<string, int> SemitoneMap = new()
    {
        { "C", 0 }, { "C#", 1 }, { "Db", 1 }, { "D", 2 }, { "D#", 3 }, { "Eb", 3 },
        { "E", 4 }, { "F", 5 }, { "F#", 6 }, { "Gb", 6 }, { "G", 7 }, { "G#", 8 },
        { "Ab", 8 }, { "A", 9 }, { "A#", 10 }, { "Bb", 10 }, { "B", 11 }
    };

    /// <summary>
    /// Определяет ближайшую ноту и отклонение в центах для заданной частоты.
    /// </summary>
    public static (string Note, float Cents) FrequencyToNote(float freq, float referenceA = 440f)
    {
        if (freq <= 0) return ("—", 0f);

        double semitones = 12.0 * Math.Log2(freq / referenceA) + 69.0;
        int rounded = (int)Math.Round(semitones);
        int noteIndex = ((rounded % 12) + 12) % 12;
        float cents = (float)((semitones - rounded) * 100.0);

        return (NoteNames[noteIndex], cents);
    }

    /// <summary>
    /// Возвращает частоту ноты по её имени (напр. "E2", "A4").
    /// </summary>
    public static float NoteToFrequency(string noteName, float referenceA = 440f)
    {
        if (string.IsNullOrEmpty(noteName) || noteName.Length < 2)
            return 0f;

        string note = noteName[..^1];
        if (!int.TryParse(noteName[^1..], out int octave)) return 0f;
        if (!SemitoneMap.TryGetValue(note, out int semitone)) return 0f;

        int midi = (octave + 1) * 12 + semitone;
        return referenceA * MathF.Pow(2f, (midi - 69) / 12f);
    }

    /// <summary>
    /// Извлекает имя ноты без октавы из полного имени (напр. "E2" → "E").
    /// </summary>
    public static string StripOctave(string noteName)
    {
        if (string.IsNullOrEmpty(noteName) || noteName.Length < 2)
            return noteName ?? "";
        return noteName[..^1];
    }
}
