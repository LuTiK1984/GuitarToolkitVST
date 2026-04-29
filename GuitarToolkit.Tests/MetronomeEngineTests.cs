using GuitarToolkit.Core.Services;
using Xunit;

namespace GuitarToolkit.Tests;

public class MetronomeEngineTests
{
    [Theory]
    [InlineData(10, 30)]
    [InlineData(120, 120)]
    [InlineData(500, 300)]
    public void BPM_ClampsToSupportedRange(int input, int expected)
    {
        var engine = new MetronomeEngine();

        engine.BPM = input;

        Assert.Equal(expected, engine.BPM);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(4, 4)]
    [InlineData(16, 8)]
    public void BeatsPerMeasure_ClampsToSupportedRange(int input, int expected)
    {
        var engine = new MetronomeEngine();

        engine.BeatsPerMeasure = input;

        Assert.Equal(expected, engine.BeatsPerMeasure);
    }

    [Theory]
    [InlineData(-1f, 0f)]
    [InlineData(0.5f, 0.5f)]
    [InlineData(2f, 1f)]
    public void Volume_ClampsToSupportedRange(float input, float expected)
    {
        var engine = new MetronomeEngine();

        engine.Volume = input;

        Assert.Equal(expected, engine.Volume);
    }

    [Fact]
    public void StartStop_UpdatesRunningState()
    {
        var engine = new MetronomeEngine();

        engine.Start();
        Assert.True(engine.IsRunning);

        engine.Stop();
        Assert.False(engine.IsRunning);
    }

    [Fact]
    public void ProcessBlock_WhenRunning_GeneratesClickAtStart()
    {
        var engine = new MetronomeEngine();
        float[] output = new float[2048];

        engine.Initialize(44100);
        engine.Start();
        engine.ProcessBlock(output, output.Length);

        Assert.Contains(output, sample => MathF.Abs(sample) > 0.0001f);
    }

    [Fact]
    public void ProcessBlock_WhenStopped_LeavesBufferUnchanged()
    {
        var engine = new MetronomeEngine();
        float[] output = Enumerable.Repeat(0.25f, 512).ToArray();

        engine.Initialize(44100);
        engine.ProcessBlock(output, output.Length);

        Assert.All(output, sample => Assert.Equal(0.25f, sample));
    }
}
