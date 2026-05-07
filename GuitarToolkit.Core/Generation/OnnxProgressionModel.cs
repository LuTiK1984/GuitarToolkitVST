using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace GuitarToolkit.Core.Generation;

public sealed class OnnxProgressionModel : IProgressionNextTokenModel, IDisposable
{
    private readonly ProgressionVocabulary _vocabulary;
    private InferenceSession? _session;
    private DateTime _loadedWriteTimeUtc;
    private string? _loadError;

    public OnnxProgressionModel(string modelPath)
        : this(modelPath, ProgressionVocabulary.Default)
    {
    }

    public OnnxProgressionModel(string modelPath, ProgressionVocabulary vocabulary)
    {
        ModelPath = modelPath;
        _vocabulary = vocabulary;
    }

    public string ModelPath { get; }

    public ProgressionModelOutput PredictNext(ProgressionModelInput input)
    {
        if (string.IsNullOrWhiteSpace(ModelPath) || !File.Exists(ModelPath))
        {
            return new ProgressionModelOutput
            {
                ModelName = "ProgressionNextTokenModel.onnx",
                IsAvailable = false,
                Status = "ONNX-файл модели не найден. Используется встроенный генератор."
            };
        }

        if (!EnsureSession())
        {
            return new ProgressionModelOutput
            {
                ModelName = Path.GetFileName(ModelPath),
                IsAvailable = false,
                Status = $"ONNX-модель не загрузилась: {_loadError}"
            };
        }

        try
        {
            int styleId = _vocabulary.GetIdOrUnknown(ToStyleToken(input.Style));
            int modeId = _vocabulary.GetIdOrUnknown(ToModeToken(input.Mode));
            int moodId = _vocabulary.GetIdOrUnknown(ToMoodToken(input.Mood));
            long[] previousIds = BuildPreviousTokenIds(input.PreviousRomanNumerals);

            using var results = _session!.Run(new[]
            {
                NamedOnnxValue.CreateFromTensor("style_id", new DenseTensor<long>(new[] { (long)styleId }, new[] { 1 })),
                NamedOnnxValue.CreateFromTensor("mode_id", new DenseTensor<long>(new[] { (long)modeId }, new[] { 1 })),
                NamedOnnxValue.CreateFromTensor("mood_id", new DenseTensor<long>(new[] { (long)moodId }, new[] { 1 })),
                NamedOnnxValue.CreateFromTensor("previous_tokens", new DenseTensor<long>(previousIds, new[] { 1, previousIds.Length }))
            });

            var logits = results.First(item => item.Name == "next_token_logits").AsTensor<float>();

            return new ProgressionModelOutput
            {
                ModelName = Path.GetFileName(ModelPath),
                IsAvailable = true,
                Status = $"ONNX-модель загружена: {Path.GetFileName(ModelPath)}",
                NextTokenProbabilities = BuildProbabilities(logits)
            };
        }
        catch (Exception ex) when (ex is OnnxRuntimeException or InvalidOperationException or ArgumentException)
        {
            return new ProgressionModelOutput
            {
                ModelName = Path.GetFileName(ModelPath),
                IsAvailable = false,
                Status = $"ONNX-inference не сработал: {ex.Message}"
            };
        }
    }

    public void Dispose()
    {
        _session?.Dispose();
        _session = null;
    }

    private bool EnsureSession()
    {
        DateTime writeTimeUtc = File.GetLastWriteTimeUtc(ModelPath);
        if (_session != null && writeTimeUtc == _loadedWriteTimeUtc)
            return true;

        try
        {
            _session?.Dispose();
            byte[] modelBytes = File.ReadAllBytes(ModelPath);
            _session = new InferenceSession(modelBytes);
            _loadedWriteTimeUtc = writeTimeUtc;
            _loadError = null;
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or OnnxRuntimeException or InvalidOperationException)
        {
            _session?.Dispose();
            _session = null;
            _loadedWriteTimeUtc = default;
            _loadError = ex.Message;
            return false;
        }
    }

    private long[] BuildPreviousTokenIds(IReadOnlyList<string> previousRomanNumerals)
    {
        var tokens = new List<string> { "<BOS>" };
        tokens.AddRange(previousRomanNumerals.Where(token => !string.IsNullOrWhiteSpace(token)));

        const int maxSequenceLength = 16;
        if (tokens.Count > maxSequenceLength)
        {
            tokens = tokens
                .Skip(tokens.Count - maxSequenceLength)
                .ToList();
            tokens[0] = "<BOS>";
        }

        return tokens
            .Select(token => (long)_vocabulary.GetIdOrUnknown(token.Trim()))
            .ToArray();
    }

    private IReadOnlyList<ModelTokenProbability> BuildProbabilities(Tensor<float> logits)
    {
        var tokenLogits = _vocabulary.ProgressionTokens
            .Select(token => new
            {
                Token = token,
                Id = _vocabulary.GetIdOrUnknown(token)
            })
            .Select(item => new
            {
                item.Token,
                Logit = (double)logits[0, item.Id]
            })
            .ToArray();

        double max = tokenLogits.Max(item => item.Logit);
        var weighted = tokenLogits
            .Select(item => new
            {
                item.Token,
                Weight = Math.Exp(item.Logit - max)
            })
            .ToArray();
        double total = weighted.Sum(item => item.Weight);

        return weighted
            .Select(item => new ModelTokenProbability(item.Token, total <= 0 ? 0 : item.Weight / total))
            .OrderByDescending(item => item.Probability)
            .ToArray();
    }

    private static string ToStyleToken(string style)
    {
        if (style.Contains("rock", StringComparison.OrdinalIgnoreCase))
            return "STYLE_ROCK";
        if (style.Contains("pop", StringComparison.OrdinalIgnoreCase))
            return "STYLE_POP";
        if (style.Contains("ambient", StringComparison.OrdinalIgnoreCase))
            return "STYLE_AMBIENT";
        if (style.Contains("blues", StringComparison.OrdinalIgnoreCase))
            return "STYLE_BLUES";
        return "STYLE_METAL";
    }

    private static string ToModeToken(string mode)
    {
        if (mode.Contains("harmonic", StringComparison.OrdinalIgnoreCase))
            return "MODE_HARMONIC_MINOR";
        if (mode.Contains("dorian", StringComparison.OrdinalIgnoreCase))
            return "MODE_DORIAN";
        if (mode.Contains("phrygian", StringComparison.OrdinalIgnoreCase))
            return "MODE_PHRYGIAN";
        if (mode.Contains("major", StringComparison.OrdinalIgnoreCase))
            return "MODE_MAJOR";
        return "MODE_NATURAL_MINOR";
    }

    private static string ToMoodToken(string mood)
    {
        if (mood.Contains("epic", StringComparison.OrdinalIgnoreCase))
            return "MOOD_EPIC";
        if (mood.Contains("bright", StringComparison.OrdinalIgnoreCase))
            return "MOOD_BRIGHT";
        if (mood.Contains("calm", StringComparison.OrdinalIgnoreCase))
            return "MOOD_CALM";
        if (mood.Contains("tense", StringComparison.OrdinalIgnoreCase))
            return "MOOD_TENSE";
        return "MOOD_DARK";
    }
}
