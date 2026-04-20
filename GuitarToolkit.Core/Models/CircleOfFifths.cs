namespace GuitarToolkit.Core.Models;

/// <summary>
/// Данные для круга квинт.
/// </summary>
public static class CircleOfFifths
{
    // 12 тональностей по кругу квинт (по часовой стрелке)
    public static readonly string[] MajorKeys =
        { "C", "G", "D", "A", "E", "B", "F#", "Db", "Ab", "Eb", "Bb", "F" };

    // Параллельные миноры
    public static readonly string[] MinorKeys =
        { "Am", "Em", "Bm", "F#m", "C#m", "G#m", "D#m", "Bbm", "Fm", "Cm", "Gm", "Dm" };

    // Количество знаков (+ = диезы, - = бемоли)
    public static readonly int[] KeySignatures =
        { 0, 1, 2, 3, 4, 5, 6, -5, -4, -3, -2, -1 };

    /// <summary>
    /// Возвращает текстовое описание ключевых знаков.
    /// </summary>
    public static string GetKeySignatureText(int index)
    {
        int signs = KeySignatures[index];
        if (signs == 0) return "Без знаков";
        if (signs > 0) return $"{signs}♯";
        return $"{-signs}♭";
    }

    /// <summary>
    /// Возвращает диатонические аккорды для позиции в круге.
    /// </summary>
    public static ProgressionStep[] GetChords(int index)
    {
        return ProgressionBuilder.GetDiatonicChords(MajorKeys[index]);
    }
}
