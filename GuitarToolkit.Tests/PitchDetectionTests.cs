using GuitarToolkit.Core.DSP;
using GuitarToolkit.Core.Models;
using GuitarToolkit.Core.Services;
using Xunit;

namespace GuitarToolkit.Tests;

/// <summary>
/// Тесты определения высоты тона.
/// Генерируем синусоиду известной частоты → прогоняем через PitchDetector/TunerEngine → проверяем результат.
/// </summary>
public class PitchDetectionTests
{
    private const int SampleRate = 44100;
    private const int FftSize = 4096;

    /// <summary>
    /// Генерирует чистую синусоиду заданной частоты.
    /// </summary>
    private static float[] GenerateSine(float frequency, int samples, float amplitude = 0.5f)
    {
        float[] buffer = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SampleRate;
            buffer[i] = amplitude * MathF.Sin(2f * MathF.PI * frequency * t);
        }
        return buffer;
    }

    /// <summary>
    /// Генерирует сигнал с гармониками (как настоящая гитарная струна).
    /// </summary>
    private static float[] GenerateGuitarLike(float frequency, int samples, float amplitude = 0.5f)
    {
        float[] buffer = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SampleRate;
            buffer[i] = amplitude * (
                MathF.Sin(2f * MathF.PI * frequency * t) * 1.0f +
                MathF.Sin(2f * MathF.PI * frequency * 2f * t) * 0.5f +
                MathF.Sin(2f * MathF.PI * frequency * 3f * t) * 0.25f +
                MathF.Sin(2f * MathF.PI * frequency * 4f * t) * 0.125f
            ) / 1.875f;
        }
        return buffer;
    }

    // ── PitchDetector — чистые синусоиды ─────────────────────

    [Theory]
    [InlineData(82.41f,  "E")]    // E2 — 6-я струна
    [InlineData(110.00f, "A")]    // A2 — 5-я струна
    [InlineData(146.83f, "D")]    // D3 — 4-я струна
    [InlineData(196.00f, "G")]    // G3 — 3-я струна
    [InlineData(246.94f, "B")]    // B3 — 2-я струна
    [InlineData(329.63f, "E")]    // E4 — 1-я струна
    [InlineData(440.00f, "A")]    // A4 — эталон
    public void PitchDetector_PureSine_DetectsCorrectNote(float frequency, string expectedNote)
    {
        var detector = new PitchDetector(FftSize, SampleRate);
        float[] samples = GenerateGuitarLike(frequency, FftSize);

        float detected = detector.DetectPitch(samples);

        // Проверяем что частота определена в пределах ±5%
        Assert.True(detected > 0, $"Не определена частота для {frequency} Hz");
        float error = MathF.Abs(detected - frequency) / frequency * 100f;
        Assert.True(error < 5f, $"Ожидалось ~{frequency} Hz, получено {detected:F1} Hz (ошибка {error:F1}%)");

        // Проверяем что нота правильная
        var (note, cents) = NoteUtils.FrequencyToNote(detected);
        Assert.Equal(expectedNote, note);
    }

    // ── PitchDetector — сигнал с гармониками ─────────────────

    [Theory]
    [InlineData(82.41f,  "E")]
    [InlineData(110.00f, "A")]
    [InlineData(146.83f, "D")]
    [InlineData(196.00f, "G")]
    [InlineData(246.94f, "B")]
    [InlineData(329.63f, "E")]
    [InlineData(440.00f, "A")]
    public void PitchDetector_WithHarmonics_DetectsCorrectNote(float frequency, string expectedNote)
    {
        var detector = new PitchDetector(FftSize, SampleRate);
        float[] samples = GenerateGuitarLike(frequency, FftSize);

        float detected = detector.DetectPitch(samples);

        Assert.True(detected > 0, $"Не определена частота для {frequency} Hz (с гармониками)");

        var (note, _) = NoteUtils.FrequencyToNote(detected);
        Assert.Equal(expectedNote, note);
    }

    // ── TunerEngine — полный конвейер ────────────────────────

    [Theory]
    [InlineData(82.41f,  "E")]
    [InlineData(110.00f, "A")]
    [InlineData(146.83f, "D")]
    [InlineData(440.00f, "A")]
    public void TunerEngine_FullPipeline_DetectsNote(float frequency, string expectedNote)
    {
        var engine = new TunerEngine(FftSize, SampleRate);
        engine.Gain = 1f;
        engine.SilenceThreshold = 0.001f;
        engine.StableThreshold = 1;

        // Подаём несколько буферов чтобы заполнить кольцевой буфер
        float[] signal = GenerateGuitarLike(frequency, FftSize * 3);

        string? detectedNote = null;
        engine.NoteDetected += (note, freq, cents) => { detectedNote = note; };

        // Подаём порциями по 1024 сэмпла (как в реальном потоке)
        int chunkSize = 1024;
        for (int i = 0; i < signal.Length; i += chunkSize)
        {
            int count = Math.Min(chunkSize, signal.Length - i);
            float[] chunk = signal[i..(i + count)];
            engine.ProcessSamples(chunk, count);
        }

        Assert.NotNull(detectedNote);
        Assert.Equal(expectedNote, detectedNote);
    }

    // ── Тишина — не должен определять ноту ───────────────────

    [Fact]
    public void TunerEngine_Silence_NoNoteDetected()
    {
        var engine = new TunerEngine(FftSize, SampleRate);
        engine.Gain = 1f;
        engine.SilenceThreshold = 0.005f;

        float[] silence = new float[FftSize * 2];

        string? detectedNote = null;
        engine.NoteDetected += (note, freq, cents) => { detectedNote = note; };

        engine.ProcessSamples(silence, silence.Length);

        Assert.Null(detectedNote);
    }

    // ── Все ноты хроматической гаммы ─────────────────────────

    [Fact]
    public void PitchDetector_AllChromaticNotes_Correct()
    {
        var detector = new PitchDetector(FftSize, SampleRate);

        // Все 12 нот в 3-й октаве (гитарный диапазон)
        (float freq, string note)[] chromatic = new[]
        {
            (130.81f, "C"),  // C3
            (138.59f, "C#"), // C#3
            (146.83f, "D"),  // D3
            (155.56f, "D#"), // D#3
            (164.81f, "E"),  // E3
            (174.61f, "F"),  // F3
            (185.00f, "F#"), // F#3
            (196.00f, "G"),  // G3
            (207.65f, "G#"), // G#3
            (220.00f, "A"),  // A3
            (233.08f, "A#"), // A#3
            (246.94f, "B"),  // B3
        };

        int passed = 0;
        foreach (var (freq, expectedNote) in chromatic)
        {
            float[] samples = GenerateGuitarLike(freq, FftSize);
            float detected = detector.DetectPitch(samples);

            if (detected > 0)
            {
                var (note, _) = NoteUtils.FrequencyToNote(detected);
                if (note == expectedNote) passed++;
                else Assert.Fail($"Нота {expectedNote} ({freq} Hz): определено как {note} ({detected:F1} Hz)");
            }
            else
            {
                Assert.Fail($"Не определена частота для {expectedNote} ({freq} Hz)");
            }
        }

        Assert.Equal(12, passed);
    }

    // ── Точность в центах ────────────────────────────────────

    [Theory]
    [InlineData(440.00f, 0f)]     // Точно A4 → 0 центов
    [InlineData(441.27f, 5f)]     // Чуть выше A4 → ~+5 центов
    [InlineData(438.74f, -5f)]    // Чуть ниже A4 → ~-5 центов
    public void NoteUtils_CentsCalculation_Accurate(float freq, float expectedCents)
    {
        var (note, cents) = NoteUtils.FrequencyToNote(freq);

        Assert.Equal("A", note);
        Assert.True(MathF.Abs(cents - expectedCents) < 2f,
            $"Ожидалось ~{expectedCents} центов, получено {cents:F1}");
    }

    // ── Расстроенная струна ──────────────────────────────────

    [Fact]
    public void TunerEngine_DetunedString_ShowsCents()
    {
        var engine = new TunerEngine(FftSize, SampleRate);
        engine.Gain = 1f;
        engine.SilenceThreshold = 0.001f;
        engine.StableThreshold = 1;

        // A немного выше (445 Hz вместо 440) → должно показать A с положительными центами
        float detuned = 445f;
        float[] signal = GenerateGuitarLike(detuned, FftSize * 3, 0.5f);

        float lastCents = 0;
        engine.NoteDetected += (note, freq, cents) => { lastCents = cents; };

        int chunkSize = 1024;
        for (int i = 0; i < signal.Length; i += chunkSize)
        {
            int count = Math.Min(chunkSize, signal.Length - i);
            engine.ProcessSamples(signal[i..(i + count)], count);
        }

        // 445 Hz — это примерно +19.6 центов от A4
        Assert.True(lastCents > 10f, $"Ожидались положительные центы для 445 Hz, получено {lastCents:F1}");
    }
}
