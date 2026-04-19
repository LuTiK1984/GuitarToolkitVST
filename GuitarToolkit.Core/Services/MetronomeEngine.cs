using GuitarToolkit.Core.DSP;

namespace GuitarToolkit.Core.Services;

/// <summary>
/// Движок метронома: генерирует щелчки прямо в аудиобуфер (sample-accurate).
/// В отличие от Timer-версии, здесь нет таймеров — позиция отсчитывается по сэмплам.
/// </summary>
public class MetronomeEngine
{
    private int _sampleRate = 44100;
    private long _samplePos;
    private int _currentBeat;

    private float[]? _accentClick;
    private float[]? _normalClick;

    private int _bpm = 120;
    private int _beatsPerMeasure = 4;
    private bool _isRunning;
    private float _volume = 0.8f;

    // ── Свойства ─────────────────────────────────────────────
    public int BPM
    {
        get => _bpm;
        set => _bpm = Math.Clamp(value, 30, 300);
    }

    public int BeatsPerMeasure
    {
        get => _beatsPerMeasure;
        set { _beatsPerMeasure = Math.Clamp(value, 2, 8); _currentBeat = 0; }
    }

    public float Volume
    {
        get => _volume;
        set => _volume = Math.Clamp(value, 0f, 1f);
    }

    public bool IsRunning => _isRunning;

    /// <summary>
    /// Срабатывает при начале каждой доли. Параметр — номер доли (0 = сильная).
    /// ВНИМАНИЕ: вызывается из аудиопотока, в обработчике используй BeginInvoke.
    /// </summary>
    public event Action<int>? BeatTick;

    // ── Инициализация ────────────────────────────────────────
    public void Initialize(int sampleRate)
    {
        _sampleRate = sampleRate;
        _accentClick = ClickGenerator.Generate(1000f, 30, sampleRate);
        _normalClick = ClickGenerator.Generate(700f, 30, sampleRate);
    }

    public void Start()
    {
        _samplePos = 0;
        _currentBeat = 0;
        _isRunning = true;
    }

    public void Stop()
    {
        _isRunning = false;
        _samplePos = 0;
        _currentBeat = 0;
    }

    /// <summary>
    /// Микширует щелчки метронома в выходной буфер.
    /// Вызывается из Process() плагина на каждый аудиоблок.
    /// </summary>
    public void ProcessBlock(float[] output, int numSamples)
    {
        if (!_isRunning || _accentClick == null || _normalClick == null)
            return;

        int samplesPerBeat = (int)((double)_sampleRate * 60.0 / _bpm);
        if (samplesPerBeat <= 0) return;

        for (int i = 0; i < numSamples; i++)
        {
            int posInBeat = (int)(_samplePos % samplesPerBeat);

            if (posInBeat == 0)
            {
                _currentBeat = (int)((_samplePos / samplesPerBeat) % _beatsPerMeasure);
                BeatTick?.Invoke(_currentBeat);
            }

            float[] click = (_currentBeat == 0) ? _accentClick : _normalClick;
            if (posInBeat < click.Length)
            {
                output[i] += click[posInBeat] * _volume;
            }

            _samplePos++;
        }
    }
}
