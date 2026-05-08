using System.Diagnostics;
using System.Text;

namespace GuitarToolkit.MLTrainer;

internal sealed class ProcessRunner
{
    private Process? _process;

    public bool IsRunning => _process is { HasExited: false };

    public async Task<int> RunAsync(
        string fileName,
        string arguments,
        string workingDirectory,
        Action<string> onOutput,
        CancellationToken cancellationToken)
    {
        if (IsRunning)
            throw new InvalidOperationException("Process is already running.");

        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
        _process = process;

        process.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
                onOutput(e.Data);
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
                onOutput(e.Data);
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        try
        {
            await process.WaitForExitAsync(cancellationToken);
            return process.ExitCode;
        }
        catch (OperationCanceledException)
        {
            Stop();
            throw;
        }
        finally
        {
            _process = null;
        }
    }

    public void Stop()
    {
        if (!IsRunning || _process == null)
            return;

        try
        {
            _process.Kill(entireProcessTree: true);
        }
        catch (InvalidOperationException)
        {
        }
    }
}
