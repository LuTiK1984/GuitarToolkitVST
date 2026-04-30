using System.Diagnostics;

namespace GuitarToolkit.Core.Services;

public static class AppLogger
{
    private static readonly object Sync = new();

    public static string LogDirectory =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GuitarToolkit",
            "logs");

    public static string LogFilePath =>
        Path.Combine(LogDirectory, $"GuitarToolkit-{DateTime.Now:yyyy-MM-dd}.log");

    public static void Info(string message)
    {
        Write("INFO", message, null);
    }

    public static void Warning(string message, Exception? exception = null)
    {
        Write("WARN", message, exception);
    }

    public static void Error(string message, Exception? exception = null)
    {
        Write("ERROR", message, exception);
    }

    private static void Write(string level, string message, Exception? exception)
    {
        try
        {
            Directory.CreateDirectory(LogDirectory);

            string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}";
            if (exception != null)
            {
                line += Environment.NewLine + exception;
            }

            lock (Sync)
            {
                File.AppendAllText(LogFilePath, line + Environment.NewLine);
            }

            Debug.WriteLine(line);
        }
        catch
        {
            // Logging must never crash audio/plugin hosts.
        }
    }
}
