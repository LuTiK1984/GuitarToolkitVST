using System.Text.Json;
using System.Text.Json.Serialization;

namespace GuitarToolkit.Core.Generation;

public sealed class ProgressionVocabulary
{
    public const int CurrentVersion = 1;

    private readonly Dictionary<string, int> _tokenToId;

    public ProgressionVocabulary(
        int version,
        IReadOnlyList<string> styleTokens,
        IReadOnlyList<string> modeTokens,
        IReadOnlyList<string> moodTokens,
        IReadOnlyList<string> progressionTokens,
        IReadOnlyList<string> specialTokens)
    {
        Version = version;
        StyleTokens = styleTokens;
        ModeTokens = modeTokens;
        MoodTokens = moodTokens;
        ProgressionTokens = progressionTokens;
        SpecialTokens = specialTokens;
        Tokens = specialTokens
            .Concat(styleTokens)
            .Concat(modeTokens)
            .Concat(moodTokens)
            .Concat(progressionTokens)
            .ToArray();

        _tokenToId = new Dictionary<string, int>(StringComparer.Ordinal);
        for (int i = 0; i < Tokens.Count; i++)
        {
            _tokenToId[Tokens[i]] = i;
        }
    }

    public int Version { get; }

    public IReadOnlyList<string> StyleTokens { get; }

    public IReadOnlyList<string> ModeTokens { get; }

    public IReadOnlyList<string> MoodTokens { get; }

    public IReadOnlyList<string> ProgressionTokens { get; }

    public IReadOnlyList<string> SpecialTokens { get; }

    public IReadOnlyList<string> Tokens { get; }

    public int Count => Tokens.Count;

    public static ProgressionVocabulary Default { get; } = new(
        CurrentVersion,
        new[] { "STYLE_METAL", "STYLE_ROCK", "STYLE_POP", "STYLE_AMBIENT", "STYLE_BLUES" },
        new[] { "MODE_MAJOR", "MODE_NATURAL_MINOR", "MODE_DORIAN", "MODE_PHRYGIAN", "MODE_HARMONIC_MINOR" },
        new[] { "MOOD_DARK", "MOOD_EPIC", "MOOD_BRIGHT", "MOOD_CALM", "MOOD_TENSE" },
        new[] { "I", "ii", "iii", "IV", "V", "vi", "vii°", "i", "ii°", "III", "iv", "v", "VI", "VII", "bII", "bVI", "bVII" },
        new[] { "<PAD>", "<BOS>", "<EOS>", "<UNK>" });

    public bool TryGetId(string token, out int id) => _tokenToId.TryGetValue(token, out id);

    public int GetIdOrUnknown(string token)
    {
        if (TryGetId(token, out int id))
            return id;

        return _tokenToId["<UNK>"];
    }

    public bool IsProgressionToken(string token) => ProgressionTokens.Contains(token, StringComparer.Ordinal);

    public static ProgressionVocabulary Load(string path)
    {
        using var stream = File.OpenRead(path);
        var document = JsonSerializer.Deserialize<VocabularyDocument>(stream)
            ?? throw new InvalidDataException("Vocabulary file is empty or invalid.");

        return new ProgressionVocabulary(
            document.Version,
            document.StyleTokens,
            document.ModeTokens,
            document.MoodTokens,
            document.ProgressionTokens,
            document.SpecialTokens);
    }

    public void ValidateCompatible()
    {
        if (Version != CurrentVersion)
            throw new InvalidDataException($"Unsupported vocabulary version {Version}.");

        var duplicate = Tokens
            .GroupBy(token => token, StringComparer.Ordinal)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicate != null)
            throw new InvalidDataException($"Duplicate vocabulary token: {duplicate.Key}");

        foreach (string token in Default.Tokens)
        {
            if (!TryGetId(token, out int currentId) || !Default.TryGetId(token, out int expectedId) || currentId != expectedId)
                throw new InvalidDataException($"Vocabulary token order mismatch: {token}");
        }
    }

    private sealed class VocabularyDocument
    {
        public int Version { get; set; }

        [JsonPropertyName("style_tokens")]
        public string[] StyleTokens { get; set; } = Array.Empty<string>();

        [JsonPropertyName("mode_tokens")]
        public string[] ModeTokens { get; set; } = Array.Empty<string>();

        [JsonPropertyName("mood_tokens")]
        public string[] MoodTokens { get; set; } = Array.Empty<string>();

        [JsonPropertyName("progression_tokens")]
        public string[] ProgressionTokens { get; set; } = Array.Empty<string>();

        [JsonPropertyName("special_tokens")]
        public string[] SpecialTokens { get; set; } = Array.Empty<string>();
    }
}
