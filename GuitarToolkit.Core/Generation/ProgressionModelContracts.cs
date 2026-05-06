namespace GuitarToolkit.Core.Generation;

public sealed class ProgressionModelInput
{
    public string Style { get; init; } = "Metal";

    public string Mode { get; init; } = "NaturalMinor";

    public string Mood { get; init; } = "Dark";

    public IReadOnlyList<string> PreviousRomanNumerals { get; init; } = Array.Empty<string>();
}

public sealed record ModelTokenProbability(string Token, double Probability);

public sealed class ProgressionModelOutput
{
    public string ModelName { get; init; } = string.Empty;

    public bool IsAvailable { get; init; }

    public string Status { get; init; } = string.Empty;

    public IReadOnlyList<ModelTokenProbability> NextTokenProbabilities { get; init; } = Array.Empty<ModelTokenProbability>();
}

public interface IProgressionNextTokenModel
{
    ProgressionModelOutput PredictNext(ProgressionModelInput input);
}
