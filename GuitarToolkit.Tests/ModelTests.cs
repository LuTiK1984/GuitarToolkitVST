using GuitarToolkit.Core.Models;
using Xunit;

namespace GuitarToolkit.Tests;

/// <summary>
/// Тесты моделей данных: ноты, аккорды, гаммы.
/// </summary>
public class ModelTests
{
    // ── NoteUtils ────────────────────────────────────────────

    [Theory]
    [InlineData("A4", 440f)]
    [InlineData("E2", 82.41f)]
    [InlineData("E4", 329.63f)]
    [InlineData("C4", 261.63f)]
    public void NoteToFrequency_StandardNotes_Correct(string noteName, float expected)
    {
        float freq = NoteUtils.NoteToFrequency(noteName);
        float error = MathF.Abs(freq - expected);
        Assert.True(error < 1f, $"{noteName}: ожидалось {expected}, получено {freq:F2}");
    }

    [Theory]
    [InlineData(440f, "A")]
    [InlineData(261.63f, "C")]
    [InlineData(329.63f, "E")]
    public void FrequencyToNote_KnownFreqs_Correct(float freq, string expectedNote)
    {
        var (note, cents) = NoteUtils.FrequencyToNote(freq);
        Assert.Equal(expectedNote, note);
        Assert.True(MathF.Abs(cents) < 5f, $"Центы должны быть ~0, получено {cents:F1}");
    }

    [Theory]
    [InlineData("E2", "E")]
    [InlineData("F#3", "F#")]
    [InlineData("Bb4", "Bb")]
    public void StripOctave_RemovesOctaveDigit(string full, string expected)
    {
        Assert.Equal(expected, NoteUtils.StripOctave(full));
    }

    // ── Tunings ──────────────────────────────────────────────

    [Fact]
    public void Tunings_StandardTuning_Has6Strings()
    {
        var std = Tunings.All["Стандарт (EADGBe)"];
        Assert.Equal(6, std.Length);
        Assert.Equal("E2", std[0]);
        Assert.Equal("E4", std[5]);
    }

    [Fact]
    public void Tunings_AllTuningsHave6Strings()
    {
        foreach (var (name, strings) in Tunings.All)
        {
            Assert.Equal(6, strings.Length);
        }
    }

    // ── ChordLibrary ─────────────────────────────────────────

    [Theory]
    [InlineData("C", "Major")]
    [InlineData("A", "m")]
    [InlineData("G", "Major")]
    [InlineData("E", "m")]
    [InlineData("D", "7")]
    public void ChordLibrary_CommonChords_Exist(string root, string type)
    {
        var chord = ChordLibrary.Get(root, type);
        Assert.NotNull(chord);
        Assert.Equal(root, chord!.Root);
        Assert.Equal(type, chord.Type);
        Assert.Equal(6, chord.Frets.Length);
    }

    [Fact]
    public void ChordLibrary_AllRootsAndTypes_HaveAtLeastOneVoicing()
    {
        int missing = 0;
        foreach (string root in ChordLibrary.AllRoots)
        {
            foreach (string type in ChordLibrary.AllTypes)
            {
                var voicings = ChordLibrary.GetVoicings(root, type);
                if (voicings.Count == 0) missing++;
            }
        }

        // Допускаем пару пропусков (некоторые экзотические комбинации)
        Assert.True(missing < 10, $"Отсутствуют аппликатуры для {missing} аккордов");
    }

    [Theory]
    [InlineData("C", "Major")]
    [InlineData("A", "m")]
    [InlineData("E", "Major")]
    public void ChordLibrary_MultipleVoicings_Available(string root, string type)
    {
        var voicings = ChordLibrary.GetVoicings(root, type);
        Assert.True(voicings.Count >= 2, $"{root}{type}: только {voicings.Count} вариант(ов)");
    }

    [Fact]
    public void ChordDefinition_DisplayName_Correct()
    {
        Assert.Equal("C", new ChordDefinition("C", "Major", new int[6], 1).DisplayName);
        Assert.Equal("Am", new ChordDefinition("A", "m", new int[6], 1).DisplayName);
        Assert.Equal("D7", new ChordDefinition("D", "7", new int[6], 1).DisplayName);
        Assert.Equal("Fmaj7", new ChordDefinition("F", "maj7", new int[6], 1).DisplayName);
    }

    // ── ChordTheory ──────────────────────────────────────────

    [Theory]
    [InlineData("C", "Major", new[] { "C", "E", "G" })]
    [InlineData("A", "m", new[] { "A", "C", "E" })]
    [InlineData("G", "7", new[] { "G", "B", "D", "F" })]
    [InlineData("D", "sus4", new[] { "D", "G", "A" })]
    public void ChordTheory_GetNotes_Correct(string root, string type, string[] expected)
    {
        var notes = ChordTheory.GetNotes(root, type);
        Assert.Equal(expected, notes);
    }

    // ── ScaleLibrary ─────────────────────────────────────────

    [Fact]
    public void ScaleLibrary_MajorScale_Has7Notes()
    {
        var major = ScaleLibrary.All[0]; // Мажор (ионийский)
        Assert.Equal(7, major.Intervals.Length);
        Assert.Equal(0, major.Intervals[0]); // тоника
    }

    [Theory]
    [InlineData(0, 0, true)]   // C в C мажоре — да
    [InlineData(1, 0, false)]  // C# в C мажоре — нет
    [InlineData(4, 0, true)]   // E в C мажоре — да
    [InlineData(6, 0, false)]  // F# в C мажоре — нет
    [InlineData(7, 0, true)]   // G в C мажоре — да
    public void ScaleLibrary_IsInScale_MajorScale(int note, int root, bool expected)
    {
        var major = ScaleLibrary.All[0];
        Assert.Equal(expected, ScaleLibrary.IsInScale(note, root, major));
    }

    // ── IntervalTrainer ──────────────────────────────────────

    [Fact]
    public void IntervalTrainer_GeneratesValidQuestion()
    {
        var trainer = new IntervalTrainer();
        var (f1, f2) = trainer.GenerateQuestion();

        Assert.True(f1 > 0);
        Assert.True(f2 > 0);
        Assert.True(f2 >= f1); // интервал вверх
    }

    [Fact]
    public void IntervalTrainer_CorrectAnswer_UpdatesStats()
    {
        var trainer = new IntervalTrainer();
        trainer.GenerateQuestion();

        int correctSemitones = trainer.CurrentInterval.Semitones;
        bool result = trainer.CheckAnswer(correctSemitones);

        Assert.True(result);
        Assert.Equal(1, trainer.CorrectAnswers);
        Assert.Equal(1, trainer.TotalAnswers);
        Assert.Equal(100f, trainer.Accuracy);
    }

    [Fact]
    public void IntervalTrainer_WrongAnswer_UpdatesStats()
    {
        var trainer = new IntervalTrainer();
        trainer.GenerateQuestion();

        int wrongSemitones = (trainer.CurrentInterval.Semitones + 3) % 12;
        bool result = trainer.CheckAnswer(wrongSemitones);

        Assert.False(result);
        Assert.Equal(0, trainer.CorrectAnswers);
        Assert.Equal(1, trainer.TotalAnswers);
    }

    // ── ProgressionBuilder ───────────────────────────────────

    [Fact]
    public void ProgressionBuilder_CMajor_CorrectChords()
    {
        var chords = ProgressionBuilder.GetDiatonicChords("C", modeIndex: 0);

        Assert.Equal(7, chords.Length);
        Assert.Equal("C", chords[0].Root);
        Assert.Equal("Major", chords[0].ChordType);
        Assert.Equal("D", chords[1].Root);
        Assert.Equal("m", chords[1].ChordType);
        Assert.Equal("G", chords[4].Root);
        Assert.Equal("Major", chords[4].ChordType);
    }

    [Fact]
    public void ProgressionBuilder_AMinor_CorrectChords()
    {
        var chords = ProgressionBuilder.GetDiatonicChords("A", modeIndex: 1);

        Assert.Equal(7, chords.Length);
        Assert.Equal("A", chords[0].Root);
        Assert.Equal("m", chords[0].ChordType);
        Assert.Equal("C", chords[2].Root);
        Assert.Equal("Major", chords[2].ChordType);
    }
}
