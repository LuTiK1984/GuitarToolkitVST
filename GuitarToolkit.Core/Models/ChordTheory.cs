namespace GuitarToolkit.Core.Models;

/// <summary>
/// Теория аккордов: формулы, ноты, интервалы.
/// </summary>
public static class ChordTheory
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
    /// Формулы аккордов: тип → массив интервалов в полутонах.
    /// </summary>
    public static readonly Dictionary<string, int[]> Formulas = new()
    {
        { "Major", new[] { 0, 4, 7 } },
        { "m",     new[] { 0, 3, 7 } },
        { "7",     new[] { 0, 4, 7, 10 } },
        { "maj7",  new[] { 0, 4, 7, 11 } },
        { "m7",    new[] { 0, 3, 7, 10 } },
        { "sus2",  new[] { 0, 2, 7 } },
        { "sus4",  new[] { 0, 5, 7 } },
        { "dim",   new[] { 0, 3, 6 } },
        { "aug",   new[] { 0, 4, 8 } },
    };

    /// <summary>
    /// Строковые формулы для отображения.
    /// </summary>
    public static readonly Dictionary<string, string> FormulaStrings = new()
    {
        { "Major", "1 – 3 – 5" },
        { "m",     "1 – ♭3 – 5" },
        { "7",     "1 – 3 – 5 – ♭7" },
        { "maj7",  "1 – 3 – 5 – 7" },
        { "m7",    "1 – ♭3 – 5 – ♭7" },
        { "sus2",  "1 – 2 – 5" },
        { "sus4",  "1 – 4 – 5" },
        { "dim",   "1 – ♭3 – ♭5" },
        { "aug",   "1 – 3 – ♯5" },
    };

    /// <summary>
    /// Полные названия типов аккордов.
    /// </summary>
    public static readonly Dictionary<string, string> TypeNames = new()
    {
        { "Major", "Мажор" },
        { "m",     "Минор" },
        { "7",     "Доминантсептаккорд" },
        { "maj7",  "Большой мажорный септаккорд" },
        { "m7",    "Малый минорный септаккорд" },
        { "sus2",  "Sus2 (задержанная секунда)" },
        { "sus4",  "Sus4 (задержанная кварта)" },
        { "dim",   "Уменьшённый" },
        { "aug",   "Увеличенный" },
    };

    /// <summary>
    /// Возвращает ноты аккорда по тонике и типу.
    /// </summary>
    public static string[] GetNotes(string root, string type)
    {
        if (!SemitoneMap.TryGetValue(root, out int rootSemitone)) return Array.Empty<string>();
        if (!Formulas.TryGetValue(type, out int[]? formula)) return Array.Empty<string>();

        return formula.Select(i => NoteNames[(rootSemitone + i) % 12]).ToArray();
    }

    /// <summary>
    /// Строковое представление формулы.
    /// </summary>
    public static string GetFormulaString(string type)
    {
        return FormulaStrings.GetValueOrDefault(type, "?");
    }

    /// <summary>
    /// Полное название типа аккорда.
    /// </summary>
    public static string GetTypeName(string type)
    {
        return TypeNames.GetValueOrDefault(type, type);
    }
}
