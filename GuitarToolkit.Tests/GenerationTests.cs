using GuitarToolkit.Core.Generation;
using Xunit;

namespace GuitarToolkit.Tests;

public class GenerationTests
{
    [Fact]
    public void DefaultVocabulary_UsesStableTokenOrder()
    {
        var vocabulary = ProgressionVocabulary.Default;

        Assert.Equal(36, vocabulary.Count);
        Assert.Equal(0, vocabulary.GetIdOrUnknown("<PAD>"));
        Assert.Equal(1, vocabulary.GetIdOrUnknown("<BOS>"));
        Assert.Equal(7, vocabulary.GetIdOrUnknown("STYLE_AMBIENT"));
        Assert.True(vocabulary.IsProgressionToken("bVII"));
        Assert.False(vocabulary.IsProgressionToken("STYLE_METAL"));
    }

    [Fact]
    public void ProgressionService_GeneratesRequestedChordCount()
    {
        var service = new ProgressionInspirationService(
            new DemoProgressionNextTokenModel(),
            new DemoProgressionNextTokenModel());

        var result = service.Generate(new GenerationRequest
        {
            RootNote = "E",
            Mode = "NaturalMinor",
            Style = "Metal",
            Mood = "Dark",
            Bars = 4,
            Seed = 7
        });

        Assert.Equal(4, result.Chords.Count);
        Assert.Equal(4, result.RomanNumerals.Count);
        Assert.All(result.Chords, chord => Assert.False(string.IsNullOrWhiteSpace(chord.Root)));
    }

    [Fact]
    public void ProgressionService_FallsBackWhenOnnxModelIsMissing()
    {
        var missingModelPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".onnx");
        var service = new ProgressionInspirationService(
            new OnnxProgressionModel(missingModelPath),
            new DemoProgressionNextTokenModel());

        var result = service.Generate(new GenerationRequest
        {
            RootNote = "E",
            Mode = "NaturalMinor",
            Bars = 4,
            Seed = 11
        });

        Assert.False(result.UsedPrimaryModel);
        Assert.Contains("ONNX-файл модели не найден", result.ModelStatus);
        Assert.Equal(4, result.Chords.Count);
    }

    [Fact]
    public void ProgressionService_MapsBorrowedFlatTokensToPlayableChords()
    {
        var service = new ProgressionInspirationService(
            new FixedProgressionModel("bII"),
            new DemoProgressionNextTokenModel());

        var result = service.Generate(new GenerationRequest
        {
            RootNote = "E",
            Mode = "NaturalMinor",
            Bars = 2,
            Seed = 11
        });

        var borrowedChord = result.Chords[1];
        Assert.Equal("bII", borrowedChord.RomanNumeral);
        Assert.Equal("F", borrowedChord.Root);
        Assert.Equal("Major", borrowedChord.ChordType);
    }

    [Fact]
    public void TemperatureSampler_RespectsTopK()
    {
        var sampler = new TemperatureSampler();
        var probabilities = new[]
        {
            new ModelTokenProbability("I", 0.90),
            new ModelTokenProbability("V", 0.08),
            new ModelTokenProbability("vi", 0.02)
        };

        for (int i = 0; i < 20; i++)
        {
            string token = sampler.Sample(probabilities, temperature: 1.0, topK: 1, new Random(i));
            Assert.Equal("I", token);
        }
    }

    private sealed class FixedProgressionModel : IProgressionNextTokenModel
    {
        private readonly string _token;

        public FixedProgressionModel(string token)
        {
            _token = token;
        }

        public ProgressionModelOutput PredictNext(ProgressionModelInput input)
        {
            return new ProgressionModelOutput
            {
                ModelName = "Fixed",
                IsAvailable = true,
                NextTokenProbabilities = new[] { new ModelTokenProbability(_token, 1.0) }
            };
        }
    }
}
