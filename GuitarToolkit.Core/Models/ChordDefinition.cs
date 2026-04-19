namespace GuitarToolkit.Core.Models;

/// <summary>
/// Определение аккорда: аппликатура на грифе.
/// </summary>
public record ChordDefinition(
    string Root,        // "C", "D#", "Bb" и т.д.
    string Type,        // "Major", "m", "7", "maj7", "m7", "sus2", "sus4", "dim", "aug"
    int[] Frets,        // 6 значений (от 6-й к 1-й струне): -1=заглушена, 0=открытая, 1+=лад
    int BaseFret        // 1 для открытой позиции, >1 для баррэ
)
{
    /// <summary>
    /// Отображаемое имя аккорда: "C", "Am", "D7", "Fmaj7" и т.д.
    /// </summary>
    public string DisplayName
    {
        get
        {
            string suffix = Type switch
            {
                "Major" => "",
                "m"     => "m",
                "7"     => "7",
                "maj7"  => "maj7",
                "m7"    => "m7",
                "sus2"  => "sus2",
                "sus4"  => "sus4",
                "dim"   => "dim",
                "aug"   => "aug",
                _       => Type
            };
            return Root + suffix;
        }
    }

    /// <summary>
    /// Есть ли баррэ (одинаковый минимальный лад на крайних играемых струнах).
    /// </summary>
    public bool HasBarre => BaseFret > 1 || (Frets.Where(f => f > 0).GroupBy(f => f).Any(g => g.Count() >= 3));
}
