namespace GuitarToolkit.Core.Generation;

public sealed record GeneratedChord(
    string RomanNumeral,
    string Root,
    string ChordType)
{
    public string DisplayName => Root + (ChordType == "Major" ? string.Empty : ChordType);
}

public sealed class GeneratedProgression
{
    public IReadOnlyList<GeneratedChord> Chords { get; init; } = Array.Empty<GeneratedChord>();

    public IReadOnlyList<string> RomanNumerals { get; init; } = Array.Empty<string>();

    public string SuggestedScale { get; init; } = string.Empty;

    public string Explanation { get; init; } = string.Empty;

    public string GuitarHint { get; init; } = string.Empty;

    public bool UsedPrimaryModel { get; init; }

    public string ModelStatus { get; init; } = string.Empty;
}
