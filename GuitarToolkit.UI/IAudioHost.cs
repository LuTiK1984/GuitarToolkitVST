namespace GuitarToolkit.UI;

public interface IAudioPlayback
{
    int SampleRate { get; }

    void PlaySamples(float[] samples);

    void StopPlayback();
}
