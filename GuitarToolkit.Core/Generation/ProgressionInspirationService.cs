using GuitarToolkit.Core.Models;

namespace GuitarToolkit.Core.Generation;

public sealed class ProgressionInspirationService
{
    private static readonly string[] NoteNames =
        { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

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
            .Select(token => ToGeneratedChord(token, request.RootNote, diatonic))
            .ToArray();

        return new GeneratedProgression
        {
            Chords = chords,
            RomanNumerals = chords.Select(chord => chord.RomanNumeral).ToArray(),
            SuggestedScale = $"{request.RootNote} {DescribeMode(request.Mode)}",
            Explanation = "Модель возвращает не звук, а символы: римские ступени аккордов. GuitarToolkit проверяет их относительно выбранной тональности, переводит в реальные аккорды и уже своими средствами может проиграть или показать результат.",
            GuitarHint = request.Style.Contains("metal", StringComparison.OrdinalIgnoreCase)
                ? "Для метал-идеи сначала попробуй palm mute и power-chord версии, потом добавь полные аппликатуры для цвета."
                : "Начни с простых аппликатур, а когда ход станет понятен на слух, усложняй обращения и фактуру.",
            UsedPrimaryModel = usedPrimary,
            ModelStatus = usedPrimary ? "Использована основная ONNX-модель." : status
        };
    }

    private static string DescribeMode(string mode)
    {
        if (mode.Contains("harmonic", StringComparison.OrdinalIgnoreCase))
            return "гармонический минор";
        if (mode.Contains("dorian", StringComparison.OrdinalIgnoreCase))
            return "дорийский лад";
        if (mode.Contains("phrygian", StringComparison.OrdinalIgnoreCase))
            return "фригийский лад";
        if (mode.Contains("major", StringComparison.OrdinalIgnoreCase))
            return "мажор";
        if (IsMinor(mode))
            return "натуральный минор";
        return mode;
    }

    private static GeneratedChord ToGeneratedChord(string token, string rootNote, IReadOnlyList<ProgressionStep> diatonic)
    {
        string normalized = NormalizeRoman(token);
        var step = diatonic.FirstOrDefault(item => NormalizeRoman(item.Degree) == normalized)
            ?? TryBuildBorrowedChord(token, rootNote)
            ?? diatonic.FirstOrDefault(item => NormalizeRoman(item.Degree).Equals("i", StringComparison.OrdinalIgnoreCase))
            ?? diatonic[0];

        return new GeneratedChord(token, step.Root, step.ChordType);
    }

    private static ProgressionStep? TryBuildBorrowedChord(string token, string rootNote)
    {
        string clean = token.Replace("В°", "", StringComparison.Ordinal)
            .Replace("°", "", StringComparison.Ordinal)
            .Replace("+", "", StringComparison.Ordinal)
            .Trim();

        int semitone = clean.ToUpperInvariant() switch
        {
            "BII" => 1,
            "II" => 2,
            "BIII" => 3,
            "III" => 4,
            "IV" => 5,
            "BV" => 6,
            "V" => 7,
            "BVI" => 8,
            "VI" => 9,
            "BVII" => 10,
            "VII" => 11,
            _ => -1
        };

        if (semitone < 0)
            return null;

        int rootIndex = Array.IndexOf(NoteNames, rootNote);
        if (rootIndex < 0)
            rootIndex = 0;

        string qualityToken = clean.StartsWith("b", StringComparison.OrdinalIgnoreCase)
            ? clean[1..]
            : clean;
        string chordType = char.IsLower(qualityToken.FirstOrDefault(char.IsLetter)) ? "m" : "Major";
        string chordRoot = NoteNames[(rootIndex + semitone) % NoteNames.Length];
        return new ProgressionStep(token, chordRoot, chordType);
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
