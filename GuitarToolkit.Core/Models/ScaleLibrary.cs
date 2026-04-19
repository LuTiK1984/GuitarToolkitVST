namespace GuitarToolkit.Core.Models;

/// <summary>
/// Библиотека гамм и ладов.
/// </summary>
public static class ScaleLibrary
{
    public static readonly List<ScaleDefinition> All = new()
    {
        new("Мажор (ионийский)",          new[] { 0, 2, 4, 5, 7, 9, 11 }),
        new("Натуральный минор (эолийский)", new[] { 0, 2, 3, 5, 7, 8, 10 }),
        new("Пентатоника мажорная",       new[] { 0, 2, 4, 7, 9 }),
        new("Пентатоника минорная",       new[] { 0, 3, 5, 7, 10 }),
        new("Блюзовая",                   new[] { 0, 3, 5, 6, 7, 10 }),
        new("Дорийский",                  new[] { 0, 2, 3, 5, 7, 9, 10 }),
        new("Миксолидийский",             new[] { 0, 2, 4, 5, 7, 9, 10 }),
        new("Фригийский",                 new[] { 0, 1, 3, 5, 7, 8, 10 }),
        new("Лидийский",                  new[] { 0, 2, 4, 6, 7, 9, 11 }),
        new("Гармонический минор",        new[] { 0, 2, 3, 5, 7, 8, 11 }),
        new("Мелодический минор",         new[] { 0, 2, 3, 5, 7, 9, 11 }),
        new("Локрийский",                 new[] { 0, 1, 3, 5, 6, 8, 10 }),
        new("Хроматическая",              new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }),
    };

    /// <summary>
    /// Имена нот.
    /// </summary>
    public static readonly string[] NoteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    /// <summary>
    /// Имена интервалов от тоники (по индексу полутона 0–11).
    /// </summary>
    public static readonly string[] IntervalNames = { "1", "b2", "2", "b3", "3", "4", "b5", "5", "b6", "6", "b7", "7" };

    /// <summary>
    /// Полутон открытой струны стандартного строя (6→1): E A D G B E.
    /// </summary>
    public static readonly int[] StandardTuning = { 4, 9, 2, 7, 11, 4 };

    /// <summary>
    /// Проверяет, входит ли нота (полутон 0–11) в гамму от заданной тоники.
    /// </summary>
    public static bool IsInScale(int noteSemitone, int rootSemitone, ScaleDefinition scale)
    {
        int interval = (noteSemitone - rootSemitone + 12) % 12;
        return scale.Intervals.Contains(interval);
    }

    /// <summary>
    /// Возвращает интервал ноты относительно тоники (строка вроде "1", "b3", "5").
    /// </summary>
    public static string GetInterval(int noteSemitone, int rootSemitone)
    {
        int interval = (noteSemitone - rootSemitone + 12) % 12;
        return IntervalNames[interval];
    }
}
