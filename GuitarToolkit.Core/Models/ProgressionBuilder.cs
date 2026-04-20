namespace GuitarToolkit.Core.Models;

public record ProgressionStep(string Degree, string Root, string ChordType);
public record ProgressionPreset(string Name, int[] Degrees, bool IsCustom = false);

/// <summary>
/// Определение лада: название, интервалы гаммы, типы аккордов на ступенях, римские цифры.
/// </summary>
public record ModeDefinition(string Name, int[] Scale, string[] ChordTypes, string[] Roman);

public static class ProgressionBuilder
{
    private static readonly string[] NoteNames =
        { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    // ── Все лады ─────────────────────────────────────────────

    public static readonly ModeDefinition[] AllModes = new[]
    {
        new ModeDefinition("Мажор (ионийский)",
            new[] { 0, 2, 4, 5, 7, 9, 11 },
            new[] { "Major", "m", "m", "Major", "Major", "m", "dim" },
            new[] { "I", "ii", "iii", "IV", "V", "vi", "vii°" }),

        new ModeDefinition("Натуральный минор (эолийский)",
            new[] { 0, 2, 3, 5, 7, 8, 10 },
            new[] { "m", "dim", "Major", "m", "m", "Major", "Major" },
            new[] { "i", "ii°", "III", "iv", "v", "VI", "VII" }),

        new ModeDefinition("Гармонический минор",
            new[] { 0, 2, 3, 5, 7, 8, 11 },
            new[] { "m", "dim", "aug", "m", "Major", "Major", "dim" },
            new[] { "i", "ii°", "III+", "iv", "V", "VI", "vii°" }),

        new ModeDefinition("Мелодический минор",
            new[] { 0, 2, 3, 5, 7, 9, 11 },
            new[] { "m", "m", "aug", "Major", "Major", "dim", "dim" },
            new[] { "i", "ii", "III+", "IV", "V", "vi°", "vii°" }),

        new ModeDefinition("Дорийский",
            new[] { 0, 2, 3, 5, 7, 9, 10 },
            new[] { "m", "m", "Major", "Major", "m", "dim", "Major" },
            new[] { "i", "ii", "III", "IV", "v", "vi°", "VII" }),

        new ModeDefinition("Миксолидийский",
            new[] { 0, 2, 4, 5, 7, 9, 10 },
            new[] { "Major", "m", "dim", "Major", "m", "m", "Major" },
            new[] { "I", "ii", "iii°", "IV", "v", "vi", "VII" }),

        new ModeDefinition("Фригийский",
            new[] { 0, 1, 3, 5, 7, 8, 10 },
            new[] { "m", "Major", "Major", "m", "dim", "Major", "m" },
            new[] { "i", "II", "III", "iv", "v°", "VI", "vii" }),

        new ModeDefinition("Лидийский",
            new[] { 0, 2, 4, 6, 7, 9, 11 },
            new[] { "Major", "Major", "m", "dim", "Major", "m", "m" },
            new[] { "I", "II", "iii", "iv°", "V", "vi", "vii" }),

        new ModeDefinition("Локрийский",
            new[] { 0, 1, 3, 5, 6, 8, 10 },
            new[] { "dim", "Major", "m", "m", "Major", "Major", "m" },
            new[] { "i°", "II", "iii", "iv", "V", "VI", "vii" }),

        new ModeDefinition("Блюзовый мажор",
            new[] { 0, 2, 3, 4, 7, 9 },
            new[] { "7", "7", "m", "Major", "7", "m" },
            new[] { "I7", "II7", "iii", "IV", "V7", "vi" }),

        new ModeDefinition("Пентатоника минорная",
            new[] { 0, 3, 5, 7, 10 },
            new[] { "m", "Major", "m", "m", "Major" },
            new[] { "i", "III", "iv", "v", "VII" }),
    };

    // ── Пресеты ──────────────────────────────────────────────

    public static readonly List<ProgressionPreset> BuiltInPresets = new()
    {
        new("I–V–vi–IV (Pop)",              new[] { 0, 4, 5, 3 }),
        new("I–IV–V–I (Классика)",          new[] { 0, 3, 4, 0 }),
        new("I–vi–IV–V (50s Doo-Wop)",      new[] { 0, 5, 3, 4 }),
        new("I–IV–vi–V (Let It Be)",        new[] { 0, 3, 5, 4 }),
        new("vi–IV–I–V (Axis)",             new[] { 5, 3, 0, 4 }),
        new("I–V–vi–iii–IV (Canon)",        new[] { 0, 4, 5, 2, 3 }),
        new("I–iii–IV–V",                   new[] { 0, 2, 3, 4 }),
        new("ii–V–I (Джаз)",               new[] { 1, 4, 0 }),
        new("I–vi–ii–V (Турнараунд)",      new[] { 0, 5, 1, 4 }),
        new("I–IV–I–V (Кантри)",           new[] { 0, 3, 0, 4 }),
        new("I–V–IV–V (Рок)",              new[] { 0, 4, 3, 4 }),
        new("i–iv–V (Минорный)",           new[] { 0, 3, 4 }),
        new("i–VI–III–VII (Эпик)",         new[] { 0, 5, 2, 6 }),
        new("i–VII–VI–V (Андалузский)",    new[] { 0, 6, 5, 4 }),
        new("12-bar Blues",                  new[] { 0, 0, 0, 0, 3, 3, 0, 0, 4, 3, 0, 4 }),
    };

    public static readonly List<ProgressionPreset> CustomPresets = new();


    public static IReadOnlyList<string> AllRoots => NoteNames;

    /// <summary>
    /// Диатонические аккорды для заданного лада.
    /// </summary>
    public static ProgressionStep[] GetDiatonicChords(string rootNote, int modeIndex = 0)
    {
        int rootIdx = Array.IndexOf(NoteNames, rootNote);
        if (rootIdx < 0) rootIdx = 0;

        var mode = AllModes[Math.Clamp(modeIndex, 0, AllModes.Length - 1)];
        int count = mode.Scale.Length;

        var steps = new ProgressionStep[count];
        for (int i = 0; i < count; i++)
        {
            int noteIdx = (rootIdx + mode.Scale[i]) % 12;
            steps[i] = new ProgressionStep(mode.Roman[i], NoteNames[noteIdx], mode.ChordTypes[i]);
        }

        return steps;
    }

    // Обратная совместимость
    public static ProgressionStep[] GetDiatonicChords(string rootNote, bool minor)
    {
        return GetDiatonicChords(rootNote, minor ? 1 : 0);
    }

    public static List<ProgressionStep> BuildFromPreset(string rootNote, ProgressionPreset preset, int modeIndex = 0)
    {
        var diatonic = GetDiatonicChords(rootNote, modeIndex);
        return preset.Degrees.Select(d => diatonic[d % diatonic.Length]).ToList();
    }

    public static ProgressionPreset SaveCustomPreset(string name, List<ProgressionStep> steps, ProgressionStep[] diatonic)
    {
        var degrees = new List<int>();
        foreach (var step in steps)
        {
            int idx = Array.FindIndex(diatonic, d => d.Root == step.Root && d.ChordType == step.ChordType);
            degrees.Add(idx >= 0 ? idx : 0);
        }

        var preset = new ProgressionPreset(name, degrees.ToArray(), IsCustom: true);
        CustomPresets.Add(preset);
        SavePresetsToDisk();
        return preset;
    }

    private static string PresetsFilePath =>
    System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "GuitarToolkit", "custom_presets.json");

    public static void SavePresetsToDisk()
    {
        try
        {
            var dir = System.IO.Path.GetDirectoryName(PresetsFilePath)!;
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);

            var data = CustomPresets.Select(p => new { p.Name, p.Degrees }).ToArray();
            string json = System.Text.Json.JsonSerializer.Serialize(data);
            System.IO.File.WriteAllText(PresetsFilePath, json);
        }
        catch { }
    }

    public static void LoadPresetsFromDisk()
    {
        try
        {
            if (!System.IO.File.Exists(PresetsFilePath)) return;
            string json = System.IO.File.ReadAllText(PresetsFilePath);
            var data = System.Text.Json.JsonSerializer.Deserialize(
    json, typeof(System.Text.Json.JsonElement[])) as System.Text.Json.JsonElement[];
            if (data == null) return;

            CustomPresets.Clear();
            foreach (var item in data)
            {
                string name = item.GetProperty("Name").GetString() ?? "Пресет";
                int[] degrees = item.GetProperty("Degrees").EnumerateArray()
                    .Select(d => d.GetInt32()).ToArray();
                CustomPresets.Add(new ProgressionPreset(name, degrees, IsCustom: true));
            }
        }
        catch { }
    }
}
