namespace GuitarToolkit.Core.Models;

public enum ChordDifficulty { Easy, Medium, Hard }

/// <summary>
/// Определение аккорда: аппликатура на грифе.
/// </summary>
public record ChordDefinition(
    string Root,
    string Type,
    int[] Frets,        // 6 значений: -1=заглушена, 0=открытая, 1+=лад
    int BaseFret         // 1 для открытой позиции, >1 для баррэ
)
{
    public string DisplayName
    {
        get
        {
            string suffix = Type switch
            {
                "Major" => "", "m" => "m", "7" => "7", "maj7" => "maj7",
                "m7" => "m7", "sus2" => "sus2", "sus4" => "sus4",
                "dim" => "dim", "aug" => "aug", _ => Type
            };
            return Root + suffix;
        }
    }

    public ChordDifficulty Difficulty
    {
        get
        {
            bool hasOpenStrings = Frets.Any(f => f == 0);
            bool hasBarre = BaseFret > 1;
            int maxFret = Frets.Where(f => f > 0).DefaultIfEmpty(0).Max();
            int span = maxFret - Frets.Where(f => f > 0).DefaultIfEmpty(0).Min();

            if (hasOpenStrings && !hasBarre && maxFret <= 3) return ChordDifficulty.Easy;
            if (span >= 4 || maxFret >= 10) return ChordDifficulty.Hard;
            return ChordDifficulty.Medium;
        }
    }

    public string PositionLabel => BaseFret <= 1 ? "Открытая" : $"{BaseFret} лад";
}
