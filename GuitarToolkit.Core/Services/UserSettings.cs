using System.Text.Json;

namespace GuitarToolkit.Core.Services;

/// <summary>
/// Настройки пользователя. Сохраняются между сессиями в JSON.
/// </summary>
public class UserSettings
{
    // Тюнер
    public float TunerGainDb { get; set; } = 20f;
    public float ReferenceA { get; set; } = 440f;
    public int TuningIndex { get; set; } = 0;

    // Метроном
    public int BPM { get; set; } = 120;
    public int BeatsPerMeasure { get; set; } = 4;
    public float MetronomeVolume { get; set; } = 0.8f;

    // Аккорды
    public string LastChordRoot { get; set; } = "C";
    public string LastChordType { get; set; } = "Major";

    // Гаммы
    public string LastScaleRoot { get; set; } = "C";
    public int LastScaleIndex { get; set; } = 0;

    // Прогрессии
    public string LastProgressionKey { get; set; } = "C";
    public int LastModeIndex { get; set; } = 0;
    public int ProgressionBPM { get; set; } = 120;

    // Desktop
    public int LastInputDevice { get; set; } = 0;

    // Табы
    public string LastTabFilePath { get; set; } = "";
    public List<string> RecentTabFilePaths { get; set; } = new();
    public int LastTabTrackIndex { get; set; } = 0;
    public double LastTabTickPosition { get; set; } = 0d;
    public double TabVolumePercent { get; set; } = 35d;
    public double TabSpeedPercent { get; set; } = 100d;
    public bool TabAutoScroll { get; set; } = true;
    public double TabSyncOffsetMilliseconds { get; set; } = 0d;
    public bool TabSoloSelectedTrack { get; set; }
    public bool TabMuteSelectedTrack { get; set; }

    // ── Файл ─────────────────────────────────────────────────

    private static string FilePath =>
        System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GuitarToolkit", "settings.json");

    public static UserSettings Load()
    {
        try
        {
            if (!System.IO.File.Exists(FilePath)) return new UserSettings();
            string json = System.IO.File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
        }
        catch (Exception ex)
        {
            AppLogger.Warning("Failed to load user settings.", ex);
            return new UserSettings();
        }
    }

    public void Save()
    {
        try
        {
            var dir = System.IO.Path.GetDirectoryName(FilePath)!;
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(this, options);
            System.IO.File.WriteAllText(FilePath, json);
        }
        catch (Exception ex)
        {
            AppLogger.Warning("Failed to save user settings.", ex);
        }
    }
}
