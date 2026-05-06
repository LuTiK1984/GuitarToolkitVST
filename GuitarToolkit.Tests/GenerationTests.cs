using GuitarToolkit.Core.Generation;
using Xunit;

namespace GuitarToolkit.Tests;

public class GenerationTests
{
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
        Assert.Contains("ONNX model file is not configured", result.ModelStatus);
        Assert.Equal(4, result.Chords.Count);
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
}
