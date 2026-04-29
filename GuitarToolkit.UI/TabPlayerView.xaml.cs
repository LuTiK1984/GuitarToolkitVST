using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using AlphaTab;
using AlphaTab.Importer;
using AlphaTab.Midi;
using AlphaTab.Model;
using AlphaTab.Rendering.Utils;
using AlphaTab.Synth;
using Microsoft.Win32;

namespace GuitarToolkit.UI;

public partial class TabPlayerView : UserControl
{
    private Score? _score;
    private bool _isUpdatingTrackMode;
    private readonly DispatcherTimer _volumeRampTimer;
    private readonly DispatcherTimer _scrollToCursorTimer;
    private double _currentMasterVolume = 0.35d;
    private double _targetMasterVolume = 0.35d;
    private bool _playbackEventsAttached;
    private double _syncOffsetMilliseconds;
    private double? _pendingTickAfterRender;
    private bool _pendingScrollAfterRender;
    private TabScrollHandler? _tabScrollHandler;

    public ObservableCollection<Track> TracksToDisplay { get; } = new();

    public TabPlayerView()
    {
        InitializeComponent();
        _volumeRampTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
        _volumeRampTimer.Tick += VolumeRampTimer_Tick;
        _scrollToCursorTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(120) };
        _scrollToCursorTimer.Tick += ScrollToCursorTimer_Tick;
        DataContext = this;
        ApplyPlaybackSettings();
        Loaded += (_, _) => ConfigureAlphaTabPlayer();
    }

    private void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Открыть табулатуру",
            Filter = "Tab files|*.gp;*.gpx;*.gp5;*.gp4;*.gp3;*.musicxml;*.xml|Guitar Pro|*.gp;*.gpx;*.gp5;*.gp4;*.gp3|MusicXML|*.musicxml;*.xml|All files|*.*"
        };

        if (dialog.ShowDialog() != true)
            return;

        LoadTab(dialog.FileName);
    }

    private void LoadTab(string path)
    {
        try
        {
            byte[] data = File.ReadAllBytes(path);
            _score = ScoreLoader.LoadScoreFromBytes(data, new Settings());
            ConfigureAlphaTabPlayer();

            TrackCombo.ItemsSource = _score.Tracks
                .Select((track, index) => new TabTrackItem(track, index + 1))
                .ToList();
            TrackCombo.SelectedIndex = _score.Tracks.Count > 0 ? 0 : -1;
            ApplyPlaybackSettings();
            ApplyTrackPlaybackMode();

            StatusText.Text = $"{Path.GetFileName(path)}";
        }
        catch (Exception ex)
        {
            TracksToDisplay.Clear();
            TrackCombo.ItemsSource = null;
            StatusText.Text = "Файл не загружен";
            MessageBox.Show(
                $"Не удалось открыть табулатуру.\n\n{ex.Message}",
                "GuitarToolkit",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    private void TrackCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TracksToDisplay.Clear();

        if (TrackCombo.SelectedItem is TabTrackItem item)
        {
            _pendingTickAfterRender = AlphaTab.Api?.TickPosition;
            _pendingScrollAfterRender = true;
            TracksToDisplay.Add(item.Track);
            ConfigureAlphaTabPlayer();
            AlphaTab.RenderTracks();
            ApplyPlaybackSettings();
            ApplyTrackPlaybackMode();
            StatusText.Text = $"Дорожка: {item}";
        }
    }

    private void Play_Click(object sender, RoutedEventArgs e)
    {
        if (_score == null) return;
        AlphaTab.Api?.Play();
    }

    private void Pause_Click(object sender, RoutedEventArgs e)
    {
        if (_score == null) return;
        AlphaTab.Api?.Pause();
    }

    private void Stop_Click(object sender, RoutedEventArgs e)
    {
        if (_score == null) return;
        AlphaTab.Api?.Stop();
    }

    private void TabScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        TabScrollViewer.ScrollToVerticalOffset(TabScrollViewer.VerticalOffset - e.Delta);
        e.Handled = true;
    }

    private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (VolumeLabel != null)
        {
            VolumeLabel.Text = $"{Math.Round(e.NewValue)}%";
        }

        ApplyPlaybackSettings();
    }

    private void SpeedCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyPlaybackSettings();
    }

    private void SyncOffsetSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _syncOffsetMilliseconds = Math.Round(e.NewValue);

        if (SyncOffsetLabel != null)
        {
            SyncOffsetLabel.Text = $"{_syncOffsetMilliseconds:+0;-0;0} мс";
        }

        ConfigureAlphaTabPlayer();
    }

    private void SpeedDown_Click(object sender, RoutedEventArgs e)
    {
        SetPlaybackSpeedPercent(GetPlaybackSpeedPercent() - 5d);
    }

    private void SpeedUp_Click(object sender, RoutedEventArgs e)
    {
        SetPlaybackSpeedPercent(GetPlaybackSpeedPercent() + 5d);
    }

    private void SpeedTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        SetPlaybackSpeedPercent(GetPlaybackSpeedPercent());
    }

    private void SpeedTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
            return;

        SetPlaybackSpeedPercent(GetPlaybackSpeedPercent());
        Keyboard.ClearFocus();
        e.Handled = true;
    }

    private void SoloToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingTrackMode)
            return;

        if (SoloToggle.IsChecked == true && MuteToggle.IsChecked == true)
        {
            _isUpdatingTrackMode = true;
            MuteToggle.IsChecked = false;
            _isUpdatingTrackMode = false;
        }

        ApplyTrackPlaybackMode();
    }

    private void MuteToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingTrackMode)
            return;

        if (MuteToggle.IsChecked == true && SoloToggle.IsChecked == true)
        {
            _isUpdatingTrackMode = true;
            SoloToggle.IsChecked = false;
            _isUpdatingTrackMode = false;
        }

        ApplyTrackPlaybackMode();
    }

    private void ApplyPlaybackSettings()
    {
        var api = AlphaTab?.Api;
        if (api == null)
            return;

        double volume = Math.Clamp((VolumeSlider?.Value ?? 35d) / 100d, 0d, 1d);
        SetMasterVolumeSmoothly(volume);

        api.PlaybackSpeed = GetSelectedPlaybackSpeed();
    }

    private void ApplyTrackPlaybackMode()
    {
        var api = AlphaTab?.Api;
        if (api == null || _score == null)
            return;

        api.ChangeTrackSolo(_score.Tracks, false);
        api.ChangeTrackMute(_score.Tracks, false);

        Track? selectedTrack = (TrackCombo?.SelectedItem as TabTrackItem)?.Track;
        if (selectedTrack == null)
            return;

        var selectedTracks = new[] { selectedTrack };
        if (SoloToggle?.IsChecked == true)
        {
            api.ChangeTrackSolo(selectedTracks, true);
        }
        else if (MuteToggle?.IsChecked == true)
        {
            api.ChangeTrackMute(selectedTracks, true);
        }
    }

    private void ConfigureAlphaTabPlayer()
    {
        var api = AlphaTab?.Api;
        if (api == null)
            return;

        api.Settings.Player.EnableCursor = true;
        api.Settings.Player.EnableAnimatedBeatCursor = true;
        api.Settings.Player.ScrollMode = ScrollMode.Smooth;
        api.Settings.Player.ScrollSpeed = 350d;
        api.Settings.Player.ScrollOffsetY = 90d;

        _tabScrollHandler ??= new TabScrollHandler(
            TabScrollViewer,
            () => AutoScrollCheckBox?.IsChecked == true,
            () => _syncOffsetMilliseconds);
        api.CustomScrollHandler = _tabScrollHandler;
        api.UpdateSettings();

        if (!_playbackEventsAttached)
        {
            api.PlayedBeatChanged.On(HandlePlayedBeatChanged);
            api.PlayerPositionChanged.On(HandlePlayerPositionChanged);
            api.PostRenderFinished.On(HandlePostRenderFinished);
            _playbackEventsAttached = true;
        }
    }

    private void HandlePlayedBeatChanged(Beat beat)
    {
        ScrollToCursorSoon();
    }

    private void HandlePlayerPositionChanged(PositionChangedEventArgs e)
    {
        _tabScrollHandler?.SetCurrentPosition(e.CurrentTime, e.CurrentTick);
    }

    private void HandlePostRenderFinished()
    {
        Dispatcher.BeginInvoke(() =>
        {
            var api = AlphaTab?.Api;
            if (api == null)
                return;

            if (_pendingTickAfterRender.HasValue)
            {
                api.TickPosition = Math.Clamp(_pendingTickAfterRender.Value, 0d, api.EndTick);
                _pendingTickAfterRender = null;
            }

            if (_pendingScrollAfterRender)
            {
                _pendingScrollAfterRender = false;
                ScrollToCursorSoon();
            }
        }, DispatcherPriority.Background);
    }

    private void ScrollToCursorSoon()
    {
        if (AutoScrollCheckBox?.IsChecked != true)
            return;

        double delay = Math.Clamp(120d + _syncOffsetMilliseconds, 0d, 400d);
        _scrollToCursorTimer.Interval = TimeSpan.FromMilliseconds(delay);

        if (!_scrollToCursorTimer.IsEnabled)
        {
            _scrollToCursorTimer.Start();
        }
    }

    private void ScrollToCursorTimer_Tick(object? sender, EventArgs e)
    {
        _scrollToCursorTimer.Stop();

        if (AutoScrollCheckBox?.IsChecked == true)
        {
            AlphaTab?.Api?.ScrollToCursor();
        }
    }

    private double GetSelectedPlaybackSpeed()
    {
        return GetPlaybackSpeedPercent() / 100d;
    }

    private double GetPlaybackSpeedPercent()
    {
        string raw = SpeedTextBox?.Text?.Trim().TrimEnd('%') ?? "100";
        raw = raw.Replace(',', '.');

        if (!double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out double percent))
            percent = 100d;

        return Math.Clamp(percent, 25d, 200d);
    }

    private void SetPlaybackSpeedPercent(double percent)
    {
        percent = Math.Clamp(Math.Round(percent), 25d, 200d);

        if (SpeedTextBox != null)
        {
            SpeedTextBox.Text = percent.ToString(CultureInfo.InvariantCulture);
        }

        ApplyPlaybackSettings();
    }

    private void SetMasterVolumeSmoothly(double volume)
    {
        _targetMasterVolume = volume;

        if (!_volumeRampTimer.IsEnabled)
        {
            _volumeRampTimer.Start();
        }
    }

    private void VolumeRampTimer_Tick(object? sender, EventArgs e)
    {
        var api = AlphaTab?.Api;
        if (api == null)
        {
            _volumeRampTimer.Stop();
            return;
        }

        double difference = _targetMasterVolume - _currentMasterVolume;
        if (Math.Abs(difference) <= 0.005d)
        {
            _currentMasterVolume = _targetMasterVolume;
            api.MasterVolume = _currentMasterVolume;
            _volumeRampTimer.Stop();
            return;
        }

        _currentMasterVolume += Math.Sign(difference) * Math.Min(Math.Abs(difference), 0.025d);
        api.MasterVolume = _currentMasterVolume;
    }

    private sealed class TabTrackItem
    {
        public TabTrackItem(Track track, int number)
        {
            Track = track;
            Number = number;
        }

        public Track Track { get; }
        public int Number { get; }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Track.Name)
                ? $"Дорожка {Number}"
                : Track.Name;
        }
    }

    private sealed class TabScrollHandler : IScrollHandler
    {
        private readonly ScrollViewer _scrollViewer;
        private readonly Func<bool> _isEnabled;
        private readonly Func<double> _syncOffsetMilliseconds;
        private double _currentTime;
        private double _currentTick;
        private DateTime _lastScrollUtc = DateTime.MinValue;

        public TabScrollHandler(
            ScrollViewer scrollViewer,
            Func<bool> isEnabled,
            Func<double> syncOffsetMilliseconds)
        {
            _scrollViewer = scrollViewer;
            _isEnabled = isEnabled;
            _syncOffsetMilliseconds = syncOffsetMilliseconds;
        }

        public void SetCurrentPosition(double currentTime, double currentTick)
        {
            _currentTime = currentTime;
            _currentTick = currentTick;
        }

        public void ForceScrollTo(BeatBounds currentBeatBounds)
        {
            ScrollToBeat(currentBeatBounds, true);
        }

        public void OnBeatCursorUpdating(
            BeatBounds startBeat,
            BeatBounds? endBeat,
            MidiTickLookupFindBeatResultCursorMode cursorMode,
            double actualBeatCursorStartX,
            double actualBeatCursorEndX,
            double actualBeatCursorTransitionDuration)
        {
            double offset = _syncOffsetMilliseconds();
            if (offset < 0 && endBeat != null)
            {
                ScrollToBeat(endBeat, false);
                return;
            }

            ScrollToBeat(startBeat, false);
        }

        public void Dispose()
        {
        }

        private void ScrollToBeat(BeatBounds? beatBounds, bool force)
        {
            if (beatBounds == null || !_isEnabled())
                return;

            if (!force && (DateTime.UtcNow - _lastScrollUtc).TotalMilliseconds < 140)
                return;

            _lastScrollUtc = DateTime.UtcNow;

            _scrollViewer.Dispatcher.BeginInvoke(() =>
            {
                var bounds = beatBounds.VisualBounds;
                double top = bounds.Y;
                double bottom = bounds.Y + bounds.H;
                double viewportTop = _scrollViewer.VerticalOffset;
                double viewportBottom = viewportTop + _scrollViewer.ViewportHeight;

                if (force || top < viewportTop + 80d || bottom > viewportBottom - 120d)
                {
                    double targetY = Math.Max(0d, top - 120d);
                    _scrollViewer.ScrollToVerticalOffset(targetY);
                }

                double left = bounds.X;
                double right = bounds.X + bounds.W;
                double viewportLeft = _scrollViewer.HorizontalOffset;
                double viewportRight = viewportLeft + _scrollViewer.ViewportWidth;

                if (force || left < viewportLeft + 40d || right > viewportRight - 120d)
                {
                    double targetX = Math.Max(0d, left - 80d);
                    _scrollViewer.ScrollToHorizontalOffset(targetX);
                }
            }, DispatcherPriority.Background);
        }
    }
}
