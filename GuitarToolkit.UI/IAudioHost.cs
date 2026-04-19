namespace GuitarToolkit.UI;

/// <summary>
/// Абстракция аудио-хоста: позволяет UI воспроизводить звук
/// не зная, работает он внутри VST-плагина или standalone-приложения.
/// </summary>
public interface IAudioPlayback
{
    /// <summary>Частота дискретизации (Гц).</summary>
    int SampleRate { get; }

    /// <summary>Поставить буфер сэмплов в очередь на воспроизведение.</summary>
    void PlaySamples(float[] samples);
}
