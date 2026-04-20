namespace GuitarToolkit.Core.Models;

/// <summary>
/// Определение музыкального интервала.
/// </summary>
public record IntervalInfo(int Semitones, string Name, string ShortName);

/// <summary>
/// Логика тренажёра интервалов: генерация заданий, проверка ответов, статистика.
/// </summary>
public class IntervalTrainer
{
    public static readonly IntervalInfo[] AllIntervals = new[]
    {
        new IntervalInfo(0,  "Унисон",            "1"),
        new IntervalInfo(1,  "Малая секунда",     "м2"),
        new IntervalInfo(2,  "Большая секунда",   "б2"),
        new IntervalInfo(3,  "Малая терция",      "м3"),
        new IntervalInfo(4,  "Большая терция",    "б3"),
        new IntervalInfo(5,  "Чистая кварта",     "ч4"),
        new IntervalInfo(6,  "Тритон",            "тт"),
        new IntervalInfo(7,  "Чистая квинта",     "ч5"),
        new IntervalInfo(8,  "Малая секста",      "м6"),
        new IntervalInfo(9,  "Большая секста",    "б6"),
        new IntervalInfo(10, "Малая септима",     "м7"),
        new IntervalInfo(11, "Большая септима",   "б7"),
        new IntervalInfo(12, "Октава",            "8"),
    };

    private readonly Random _rng = new();

    // Текущее задание
    public int CurrentBaseSemitone { get; private set; }
    public int CurrentIntervalIndex { get; private set; }
    public IntervalInfo CurrentInterval => AllIntervals[CurrentIntervalIndex];

    // Статистика
    public int TotalAnswers { get; private set; }
    public int CorrectAnswers { get; private set; }
    public float Accuracy => TotalAnswers > 0 ? (float)CorrectAnswers / TotalAnswers * 100f : 0f;

    // Настройки
    public bool IncludeUnison { get; set; } = false;
    public int MaxSemitones { get; set; } = 12; // 7 = до квинты, 12 = все

    /// <summary>
    /// Генерирует новое задание. Возвращает частоты двух нот.
    /// </summary>
    public (float Freq1, float Freq2) GenerateQuestion()
    {
        // Случайная базовая нота в гитарном диапазоне (E2=82Hz .. E4=330Hz)
        // MIDI 40 (E2) .. 64 (E4)
        int baseMidi = _rng.Next(40, 60);
        CurrentBaseSemitone = baseMidi;

        // Случайный интервал
        int minInterval = IncludeUnison ? 0 : 1;
        int maxSemitones = Math.Clamp(MaxSemitones, 1, 12);
        CurrentIntervalIndex = _rng.Next(minInterval, maxSemitones + 1);

        float freq1 = MidiToFreq(baseMidi);
        float freq2 = MidiToFreq(baseMidi + CurrentInterval.Semitones);

        return (freq1, freq2);
    }

    /// <summary>
    /// Проверить ответ. Возвращает true если правильно.
    /// </summary>
    public bool CheckAnswer(int semitones)
    {
        TotalAnswers++;
        bool correct = semitones == CurrentInterval.Semitones;
        if (correct) CorrectAnswers++;
        return correct;
    }

    /// <summary>
    /// Сброс статистики.
    /// </summary>
    public void ResetStats()
    {
        TotalAnswers = 0;
        CorrectAnswers = 0;
    }

    private static float MidiToFreq(int midi)
    {
        return 440f * MathF.Pow(2f, (midi - 69) / 12f);
    }
}
