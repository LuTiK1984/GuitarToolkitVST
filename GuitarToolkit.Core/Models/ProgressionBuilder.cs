namespace GuitarToolkit.Core.Models;

public record ProgressionStep(string Degree, string Root, string ChordType);
public record ProgressionPreset(string Name, int[] Degrees, bool IsCustom = false);

public static class ProgressionBuilder
{
    private static readonly string[] NoteNames =
        { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    // Мажорная гамма
    private static readonly int[] MajorScale = { 0, 2, 4, 5, 7, 9, 11 };
    private static readonly string[] MajorChordTypes = { "Major", "m", "m", "Major", "Major", "m", "dim" };
    private static readonly string[] RomanMajor = { "I", "ii", "iii", "IV", "V", "vi", "vii°" };

    // Минорная гамма (натуральный минор)
    private static readonly int[] MinorScale = { 0, 2, 3, 5, 7, 8, 10 };
    private static readonly string[] MinorChordTypes = { "m", "dim", "Major", "m", "m", "Major", "Major" };
    private static readonly string[] RomanMinor = { "i", "ii°", "III", "iv", "v", "VI", "VII" };

    // Встроенные пресеты
    public static readonly List<ProgressionPreset> BuiltInPresets = new()
    {
        // Мажорные
        new("I–V–vi–IV (Pop)",              new[] { 0, 4, 5, 3 }),
        new("I–IV–V–I (Классика)",          new[] { 0, 3, 4, 0 }),
        new("I–vi–IV–V (50s Doo-Wop)",      new[] { 0, 5, 3, 4 }),
        new("I–IV–vi–V (Let It Be)",        new[] { 0, 3, 5, 4 }),
        new("vi–IV–I–V (Axis of Awesome)",  new[] { 5, 3, 0, 4 }),
        new("I–V–vi–iii–IV (Canon)",        new[] { 0, 4, 5, 2, 3 }),
        new("I–iii–IV–V",                   new[] { 0, 2, 3, 4 }),
        new("ii–V–I (Джаз)",               new[] { 1, 4, 0 }),
        new("I–vi–ii–V (Джаз турнараунд)", new[] { 0, 5, 1, 4 }),
        new("I–IV–I–V (Кантри)",           new[] { 0, 3, 0, 4 }),
        new("12-bar Blues",                  new[] { 0, 0, 0, 0, 3, 3, 0, 0, 4, 3, 0, 4 }),
        new("I–V–IV–V (Рок)",              new[] { 0, 4, 3, 4 }),
        new("I–bVII–IV–I (Рок миксолид.)", new[] { 0, 6, 3, 0 }),
    };

    // Пользовательские пресеты (хранятся в памяти)
    public static readonly List<ProgressionPreset> CustomPresets = new();

    public static IReadOnlyList<string> AllRoots => NoteNames;

    /// <summary>
    /// Диатонические аккорды — мажор или минор.
    /// </summary>
    public static ProgressionStep[] GetDiatonicChords(string rootNote, bool minor = false)
    {
        int rootIdx = Array.IndexOf(NoteNames, rootNote);
        if (rootIdx < 0) rootIdx = 0;

        int[] scale = minor ? MinorScale : MajorScale;
        string[] types = minor ? MinorChordTypes : MajorChordTypes;
        string[] roman = minor ? RomanMinor : RomanMajor;

        var steps = new ProgressionStep[7];
        for (int i = 0; i < 7; i++)
        {
            int noteIdx = (rootIdx + scale[i]) % 12;
            steps[i] = new ProgressionStep(roman[i], NoteNames[noteIdx], types[i]);
        }

        return steps;
    }

    public static List<ProgressionStep> BuildFromPreset(string rootNote, ProgressionPreset preset, bool minor = false)
    {
        var diatonic = GetDiatonicChords(rootNote, minor);
        return preset.Degrees.Select(d => diatonic[d % 7]).ToList();
    }

    /// <summary>
    /// Сохранить текущую прогрессию как пользовательский пресет.
    /// </summary>
    public static ProgressionPreset SaveCustomPreset(string name, List<ProgressionStep> steps, ProgressionStep[] diatonic)
    {
        // Находим индексы ступеней
        var degrees = new List<int>();
        foreach (var step in steps)
        {
            int idx = Array.FindIndex(diatonic, d => d.Root == step.Root && d.ChordType == step.ChordType);
            degrees.Add(idx >= 0 ? idx : 0);
        }

        var preset = new ProgressionPreset(name, degrees.ToArray(), IsCustom: true);
        CustomPresets.Add(preset);
        return preset;
    }
}
