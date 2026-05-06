namespace GuitarToolkit.Core.Generation;

public sealed class DemoProgressionNextTokenModel : IProgressionNextTokenModel
{
    private static readonly IReadOnlyDictionary<string, ModelTokenProbability[]> MinorMetalDark = new Dictionary<string, ModelTokenProbability[]>
    {
        ["i"] = new[]
        {
            new ModelTokenProbability("VI", 0.42),
            new ModelTokenProbability("VII", 0.32),
            new ModelTokenProbability("iv", 0.16),
            new ModelTokenProbability("bII", 0.10)
        },
        ["VI"] = new[]
        {
            new ModelTokenProbability("VII", 0.45),
            new ModelTokenProbability("III", 0.30),
            new ModelTokenProbability("iv", 0.15),
            new ModelTokenProbability("i", 0.10)
        },
        ["VII"] = new[]
        {
            new ModelTokenProbability("i", 0.60),
            new ModelTokenProbability("VI", 0.25),
            new ModelTokenProbability("iv", 0.15)
        },
        ["iv"] = new[]
        {
            new ModelTokenProbability("V", 0.45),
            new ModelTokenProbability("VII", 0.30),
            new ModelTokenProbability("i", 0.25)
        }
    };

    private static readonly ModelTokenProbability[] DefaultMinor =
    {
        new("i", 0.35),
        new("VI", 0.25),
        new("VII", 0.25),
        new("iv", 0.15)
    };

    private static readonly ModelTokenProbability[] DefaultMajor =
    {
        new("I", 0.35),
        new("V", 0.25),
        new("vi", 0.20),
        new("IV", 0.20)
    };

    public ProgressionModelOutput PredictNext(ProgressionModelInput input)
    {
        string previous = input.PreviousRomanNumerals.LastOrDefault() ?? (IsMinor(input.Mode) ? "i" : "I");
        IReadOnlyList<ModelTokenProbability> probabilities = IsMinor(input.Mode)
            ? MinorMetalDark.GetValueOrDefault(previous, DefaultMinor)
            : DefaultMajor;

        return new ProgressionModelOutput
        {
            ModelName = "DemoProgressionNextTokenModel",
            IsAvailable = true,
            Status = "Демонстрационная символическая модель. После обучения ее заменит ProgressionNextTokenModel.onnx.",
            NextTokenProbabilities = probabilities
        };
    }

    private static bool IsMinor(string mode)
    {
        return mode.Contains("minor", StringComparison.OrdinalIgnoreCase)
            || mode.Contains("aeolian", StringComparison.OrdinalIgnoreCase)
            || mode.Contains("phrygian", StringComparison.OrdinalIgnoreCase)
            || mode.Contains("dorian", StringComparison.OrdinalIgnoreCase);
    }
}
