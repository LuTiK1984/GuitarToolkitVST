namespace GuitarToolkit.Core.Models;

public static class ChordLibrary
{
    private static readonly string[] Roots = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    public static readonly string[] Types = { "Major", "m", "7", "maj7", "m7", "sus2", "sus4", "dim", "aug" };

    public static IReadOnlyList<string> AllRoots => Roots;
    public static IReadOnlyList<string> AllTypes => Types;

    // Хранилище: ключ → список вариантов аппликатуры
    private static readonly Dictionary<string, List<ChordDefinition>> _cache = new();

    // ── Избранное ────────────────────────────────────────────

    public static readonly HashSet<string> Favorites = new();

    private static string FavoritesPath =>
        System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GuitarToolkit", "favorites.json");

    public static void LoadFavorites()
    {
        try
        {
            if (!System.IO.File.Exists(FavoritesPath)) return;
            string json = System.IO.File.ReadAllText(FavoritesPath);
            var items = System.Text.Json.JsonSerializer.Deserialize(
                json, typeof(string[])) as string[];
            if (items != null)
            {
                Favorites.Clear();
                foreach (var item in items) Favorites.Add(item);
            }
        }
        catch { }
    }

    public static void SaveFavorites()
    {
        try
        {
            var dir = System.IO.Path.GetDirectoryName(FavoritesPath)!;
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            string json = System.Text.Json.JsonSerializer.Serialize(Favorites.ToArray());
            System.IO.File.WriteAllText(FavoritesPath, json);
        }
        catch { }
    }

    public static bool IsFavorite(string root, string type) => Favorites.Contains($"{root}|{type}");

    public static void ToggleFavorite(string root, string type)
    {
        string key = $"{root}|{type}";
        if (!Favorites.Remove(key)) Favorites.Add(key);
        SaveFavorites();
    }

    // ── Шаблоны ──────────────────────────────────────────────

    private static readonly Dictionary<string, int[]> EShapes = new()
    {
        { "Major", new[] { 0, 2, 2, 1, 0, 0 } },
        { "m",     new[] { 0, 2, 2, 0, 0, 0 } },
        { "7",     new[] { 0, 2, 0, 1, 0, 0 } },
        { "maj7",  new[] { 0, 2, 1, 1, 0, 0 } },
        { "m7",    new[] { 0, 2, 0, 0, 0, 0 } },
        { "sus4",  new[] { 0, 2, 2, 2, 0, 0 } },
        { "aug",   new[] { 0, 3, 2, 1, 1, 0 } },
    };

    private static readonly Dictionary<string, int[]> AShapes = new()
    {
        { "Major", new[] { -1, 0, 2, 2, 2, 0 } },
        { "m",     new[] { -1, 0, 2, 2, 1, 0 } },
        { "7",     new[] { -1, 0, 2, 0, 2, 0 } },
        { "maj7",  new[] { -1, 0, 2, 1, 2, 0 } },
        { "m7",    new[] { -1, 0, 2, 0, 1, 0 } },
        { "sus2",  new[] { -1, 0, 2, 2, 0, 0 } },
        { "sus4",  new[] { -1, 0, 2, 2, 3, 0 } },
        { "dim",   new[] { -1, 0, 1, 2, 1, -1 } },
    };

    private static readonly List<ChordDefinition> OpenOverrides = new()
    {
        new("C",  "Major", new[] { -1, 3, 2, 0, 1, 0 }, 1),
        new("C",  "m",     new[] { -1, 3, 5, 5, 4, 3 }, 3),
        new("C",  "7",     new[] { -1, 3, 2, 3, 1, 0 }, 1),
        new("C",  "maj7",  new[] { -1, 3, 2, 0, 0, 0 }, 1),

        new("D",  "Major", new[] { -1, -1, 0, 2, 3, 2 }, 1),
        new("D",  "m",     new[] { -1, -1, 0, 2, 3, 1 }, 1),
        new("D",  "7",     new[] { -1, -1, 0, 2, 1, 2 }, 1),
        new("D",  "sus2",  new[] { -1, -1, 0, 2, 3, 0 }, 1),
        new("D",  "sus4",  new[] { -1, -1, 0, 2, 3, 3 }, 1),

        new("G",  "Major", new[] { 3, 2, 0, 0, 0, 3 }, 1),
        new("G",  "7",     new[] { 3, 2, 0, 0, 0, 1 }, 1),

        new("A",  "Major", new[] { -1, 0, 2, 2, 2, 0 }, 1),
        new("A",  "m",     new[] { -1, 0, 2, 2, 1, 0 }, 1),
        new("A",  "7",     new[] { -1, 0, 2, 0, 2, 0 }, 1),
        new("A",  "maj7",  new[] { -1, 0, 2, 1, 2, 0 }, 1),
        new("A",  "m7",    new[] { -1, 0, 2, 0, 1, 0 }, 1),
        new("A",  "sus2",  new[] { -1, 0, 2, 2, 0, 0 }, 1),
        new("A",  "sus4",  new[] { -1, 0, 2, 2, 3, 0 }, 1),
        new("A",  "dim",   new[] { -1, 0, 1, 2, 1, -1 }, 1),

        new("E",  "Major", new[] { 0, 2, 2, 1, 0, 0 }, 1),
        new("E",  "m",     new[] { 0, 2, 2, 0, 0, 0 }, 1),
        new("E",  "7",     new[] { 0, 2, 0, 1, 0, 0 }, 1),
        new("E",  "maj7",  new[] { 0, 2, 1, 1, 0, 0 }, 1),
        new("E",  "m7",    new[] { 0, 2, 0, 0, 0, 0 }, 1),
        new("E",  "sus4",  new[] { 0, 2, 2, 2, 0, 0 }, 1),
        new("E",  "aug",   new[] { 0, 3, 2, 1, 1, 0 }, 1),
    };

    static ChordLibrary()
    {
        // E-формы
        foreach (var (type, shape) in EShapes)
        {
            for (int fret = 0; fret <= 11; fret++)
            {
                int rootIdx = (4 + fret) % 12;
                string root = Roots[rootIdx];
                AddVoicing(root, type, FromShape(root, type, shape, fret));
            }
        }

        // A-формы
        foreach (var (type, shape) in AShapes)
        {
            for (int fret = 0; fret <= 11; fret++)
            {
                int rootIdx = (9 + fret) % 12;
                string root = Roots[rootIdx];
                AddVoicing(root, type, FromShape(root, type, shape, fret));
            }
        }

        // Открытые (добавляются первыми в список)
        foreach (var chord in OpenOverrides)
        {
            string key = $"{chord.Root}|{chord.Type}";
            if (_cache.ContainsKey(key))
                _cache[key].Insert(0, chord);
            else
                _cache[key] = new List<ChordDefinition> { chord };
        }

        // Убираем дубликаты по Frets
        foreach (var kvp in _cache)
        {
            var unique = kvp.Value
                .GroupBy(c => string.Join(",", c.Frets))
                .Select(g => g.First())
                .OrderBy(c => c.BaseFret)
                .ToList();
            _cache[kvp.Key] = unique;
        }
    }

    private static void AddVoicing(string root, string type, ChordDefinition chord)
    {
        string key = $"{root}|{type}";
        if (!_cache.ContainsKey(key))
            _cache[key] = new List<ChordDefinition>();
        _cache[key].Add(chord);
    }

    private static ChordDefinition FromShape(string root, string type, int[] shape, int rootFret)
    {
        int[] frets = new int[6];
        for (int i = 0; i < 6; i++)
            frets[i] = shape[i] < 0 ? -1 : shape[i] + rootFret;
        return new ChordDefinition(root, type, frets, rootFret == 0 ? 1 : rootFret);
    }

    /// <summary>
    /// Первый (основной) вариант аккорда. Обратная совместимость.
    /// </summary>
    public static ChordDefinition? Get(string root, string type)
    {
        return _cache.GetValueOrDefault($"{root}|{type}")?.FirstOrDefault();
    }

    /// <summary>
    /// Все варианты аппликатуры для данного аккорда.
    /// </summary>
    public static IReadOnlyList<ChordDefinition> GetVoicings(string root, string type)
    {
        return _cache.GetValueOrDefault($"{root}|{type}") ?? new List<ChordDefinition>();
    }

    /// <summary>
    /// Все аккорды-избранные.
    /// </summary>
    public static IEnumerable<ChordDefinition> GetFavorites()
    {
        return Favorites.Select(key =>
        {
            var parts = key.Split('|');
            return parts.Length == 2 ? Get(parts[0], parts[1]) : null;
        }).Where(c => c != null)!;
    }
}
