namespace GuitarToolkit.Core.Models;

/// <summary>
/// Библиотека гитарных аккордов.
/// Генерирует аппликатуры из шаблонов E-формы и A-формы + ручные открытые аккорды.
/// </summary>
public static class ChordLibrary
{
    private static readonly string[] Roots = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    // Типы аккордов для UI
    public static readonly string[] Types = { "Major", "m", "7", "maj7", "m7", "sus2", "sus4", "dim", "aug" };

    public static IReadOnlyList<string> AllRoots => Roots;
    public static IReadOnlyList<string> AllTypes => Types;

    // ── Шаблоны форм (относительные лады) ────────────────────

    // E-форма: тоника на 6-й струне
    // Индекс 0 в шаблоне = открытая (при rootFret=0) или баррэ-лад
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

    // A-форма: тоника на 5-й струне
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

    // Открытые аккорды с уникальными аппликатурами (переопределяют сгенерированные)
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

    // Кеш всех аккордов
    private static readonly Dictionary<string, ChordDefinition> _cache = new();

    static ChordLibrary()
    {
        // 1. Генерируем из E-формы (тоника на 6-й струне)
        // E=лад 0, F=1, F#=2, G=3, G#=4, A=5, A#=6, B=7, C=8, C#=9, D=10, D#=11
        foreach (var (type, shape) in EShapes)
        {
            for (int fret = 0; fret <= 11; fret++)
            {
                int rootIdx = (4 + fret) % 12; // E = индекс 4 в массиве Roots
                string root = Roots[rootIdx];
                var chord = FromShape(root, type, shape, fret);
                string key = $"{root}|{type}";
                _cache.TryAdd(key, chord);
            }
        }

        // 2. Генерируем из A-формы (тоника на 5-й струне)
        foreach (var (type, shape) in AShapes)
        {
            for (int fret = 0; fret <= 11; fret++)
            {
                int rootIdx = (9 + fret) % 12; // A = индекс 9 в массиве Roots
                string root = Roots[rootIdx];
                string key = $"{root}|{type}";

                // A-форму добавляем только если нет E-формы с более низкой позицией
                if (!_cache.ContainsKey(key))
                    _cache[key] = FromShape(root, type, shape, fret);
                else if (_cache[key].BaseFret > (fret == 0 ? 1 : fret))
                    _cache[key] = FromShape(root, type, shape, fret);
            }
        }

        // 3. Перекрываем ручными открытыми аккордами
        foreach (var chord in OpenOverrides)
        {
            string key = $"{chord.Root}|{chord.Type}";
            _cache[key] = chord;
        }
    }

    private static ChordDefinition FromShape(string root, string type, int[] shape, int rootFret)
    {
        int[] frets = new int[6];
        for (int i = 0; i < 6; i++)
        {
            frets[i] = shape[i] < 0 ? -1 : shape[i] + rootFret;
        }
        return new ChordDefinition(root, type, frets, rootFret == 0 ? 1 : rootFret);
    }

    /// <summary>
    /// Получить аккорд по тонике и типу.
    /// </summary>
    public static ChordDefinition? Get(string root, string type)
    {
        return _cache.GetValueOrDefault($"{root}|{type}");
    }

    /// <summary>
    /// Все аккорды для данной тоники.
    /// </summary>
    public static IEnumerable<ChordDefinition> GetByRoot(string root)
    {
        return _cache.Values.Where(c => c.Root == root);
    }
}
