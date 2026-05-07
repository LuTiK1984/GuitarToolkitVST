namespace GuitarToolkit.Core.Generation;

public sealed class GenerationRequest
{
    public string RootNote { get; init; } = "E";

    public string Mode { get; init; } = "NaturalMinor";

    public string Style { get; init; } = "Metal";

    public string Mood { get; init; } = "Dark";

    public int Bars { get; init; } = 4;

    public int Difficulty { get; init; } = 2;

    public double Temperature { get; init; } = 0.85;

    public int TopK { get; init; } = 5;

    public int? Seed { get; init; }

    public IReadOnlyList<string> SeedRomanNumerals { get; init; } = Array.Empty<string>();
}
