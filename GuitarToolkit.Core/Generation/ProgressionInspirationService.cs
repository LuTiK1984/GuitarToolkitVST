using GuitarToolkit.Core.Models;

namespace GuitarToolkit.Core.Generation;

public sealed class ProgressionInspirationService
{
    private readonly IProgressionNextTokenModel _primaryModel;
    private readonly IProgressionNextTokenModel _fallbackModel;
    private readonly TemperatureSampler _sampler;

    public ProgressionInspirationService(
        IProgressionNextTokenModel primaryModel,
        IProgressionNextTokenModel? fallbackModel = null,
        TemperatureSampler? sampler = null)
    {
        _primaryModel = primaryModel;
        _fallbackModel = fallbackModel ?? new DemoProgressionNextTokenModel();
        _sampler = sampler ?? new TemperatureSampler();
    }

    public GeneratedProgression Generate(GenerationRequest request)
    {
        int length = Math.Clamp(request.Bars, 2, 16);
        var random = request.Seed.HasValue ? new Random(request.Seed.Value) : new Random();
        var tokens = request.SeedRomanNumerals.Count > 0
            ? request.SeedRomanNumerals.ToList()
            : new List<string> { IsMinor(request.Mode) ? "i" : "I" };

        bool usedPrimary = true;
        string status = string.Empty;

        while (tokens.Count < length)
        {
            var input = new ProgressionModelInput
            {
                Style = request.Style,
                Mode = request.Mode,
                Mood = request.Mood,
                PreviousRomanNumerals = tokens
            };

            var output = _primaryModel.PredictNext(input);
            if (!output.IsAvailable || output.NextTokenProbabilities.Count == 0)
            {
                usedPrimary = false;
                status = output.Status;
                output = _fallbackModel.PredictNext(input);
            }

            string token = _sampler.Sample(output.NextTokenProbabilities, request.Temperature, request.TopK, random);
            tokens.Add(token);
        }

        var diatonic = ProgressionBuilder.GetDiatonicChords(request.RootNote, ResolveModeIndex(request.Mode));
        var chords = tokens
            .Take(length)
            .Select(token => ToGeneratedChord(token, diatonic))
            .ToArray();

        return new GeneratedProgression
        {
            Chords = chords,
            RomanNumerals = chords.Select(chord => chord.RomanNumeral).ToArray(),
            SuggestedScale = $"{request.RootNote} {request.Mode}",
            Explanation = "Model tokens are converted into roman numerals, validated against the selected key/mode, then mapped to GuitarToolkit chords.",
            GuitarHint = request.Style.Contains("metal", StringComparison.OrdinalIgnoreCase)
                ? "Try palm-muted power-chord versions first, then add full voicings for color."
                : "Start with simple chord voicings and move to richer shapes when the progression feels stable.",
            UsedPrimaryModel = usedPrimary,
            ModelStatus = usedPrimary ? "Primary ONNX model was used." : status
        };
    }

    private static GeneratedChord ToGeneratedChord(string token, IReadOnlyList<ProgressionStep> diatonic)
    {
        string normalized = NormalizeRoman(token);
        var step = diatonic.FirstOrDefault(item => NormalizeRoman(item.Degree) == normalized)
            ?? diatonic.FirstOrDefault(item => NormalizeRoman(item.Degree).Equals("i", StringComparison.OrdinalIgnoreCase))
            ?? diatonic[0];

        return new GeneratedChord(token, step.Root, step.ChordType);
    }

    private static int ResolveModeIndex(string mode)
    {
        if (mode.Contains("harmonic", StringComparison.OrdinalIgnoreCase))
            return 2;
        if (mode.Contains("dorian", StringComparison.OrdinalIgnoreCase))
            return 4;
        if (mode.Contains("phrygian", StringComparison.OrdinalIgnoreCase))
            return 6;
        if (IsMinor(mode))
            return 1;
        return 0;
    }

    private static bool IsMinor(string mode)
    {
        return mode.Contains("minor", StringComparison.OrdinalIgnoreCase)
            || mode.Contains("aeolian", StringComparison.OrdinalIgnoreCase)
            || mode.Contains("dorian", StringComparison.OrdinalIgnoreCase)
            || mode.Contains("phrygian", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeRoman(string value)
    {
        return value.Replace("°", "", StringComparison.Ordinal)
            .Replace("b", "", StringComparison.OrdinalIgnoreCase)
            .Replace("+", "", StringComparison.Ordinal)
            .Trim();
    }
}
