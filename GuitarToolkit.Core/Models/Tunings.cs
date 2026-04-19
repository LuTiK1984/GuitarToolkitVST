namespace GuitarToolkit.Core.Models;

/// <summary>
/// Справочник гитарных строёв.
/// Ключ — название, значение — массив нот от 6-й до 1-й струны.
/// </summary>
public static class Tunings
{
    public static readonly Dictionary<string, string[]> All = new()
    {
        { "Стандарт (EADGBe)",  new[] { "E2", "A2", "D3", "G3", "B3", "E4" } },
        { "Drop D (DADGBe)",    new[] { "D2", "A2", "D3", "G3", "B3", "E4" } },
        { "Drop C (CGCFAd)",    new[] { "C2", "G2", "C3", "F3", "A3", "D4" } },
        { "Open G (DGDGBd)",    new[] { "D2", "G2", "D3", "G3", "B3", "D4" } },
        { "Open D (DADf#Ad)",   new[] { "D2", "A2", "D3", "F#3", "A3", "D4" } },
        { "DADGAD",             new[] { "D2", "A2", "D3", "G3", "A3", "D4" } },
        { "Half Step Down",     new[] { "Eb2", "Ab2", "Db3", "Gb3", "Bb3", "Eb4" } },
        { "Full Step Down",     new[] { "D2", "G2", "C3", "F3", "A3", "D4" } },
    };
}
