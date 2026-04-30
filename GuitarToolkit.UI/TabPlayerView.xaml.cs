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
using GuitarToolkit.Core.Services;
using Microsoft.Win32;

namespace GuitarToolkit.UI;

public partial class TabPlayerView : UserControl
{
    private const int MaxRecentTabFiles = 10;
    private const int MaxFavoriteTabFiles = 50;
    private static readonly string[] SupportedTabExtensions =
    [
        ".gp",
        ".gpx",
        ".gp5",
        ".gp4",
        ".gp3",
        ".musicxml",
        ".xml"
    ];

    private readonly UserSettings? _settings;
    private Score? _score;
    private bool _isUpdatingTrackMode;
    private bool _isInitializingSettings;
    private bool _isUpdatingFavoriteToggle;
    private bool _restoredLastFile;
    private readonly DispatcherTimer _volumeRampTimer;
    private readonly DispatcherTimer _scrollToCursorTimer;
    private readonly DispatcherTimer _resizeRenderTimer;
    private double _currentMasterVolume = 0.35d;
    private double _targetMasterVolume = 0.35d;
    private bool _playbackEventsAttached;
    private double? _pendingTickAfterRender;
    private bool _pendingScrollAfterRender;
    private TabScrollHandler? _tabScrollHandler;
    private bool _isUpdatingPlayButton;
    private bool _isUpdatingCurrentFiles;
    private int _pendingResizeRenderPasses;
    private string? _loadedFilePath;
    private string? _libraryFolderPath;
    private double _lastKnownTickPosition;

    public ObservableCollection<Track> TracksToDisplay { get; } = new();
    public ObservableCollection<TabFileItem> LibraryTabFiles { get; } = new();
    public ObservableCollection<RecentTabFileItem> RecentTabFiles { get; } = new();
    public ObservableCollection<TabFileItem> FavoriteTabFiles { get; } = new();
    public ObservableCollection<TabFileItem> CurrentTabFiles { get; } = new();

    public TabPlayerView()
        : this(null)
    {
    }

    public TabPlayerView(UserSettings? settings)
    {
        _settings = settings;
        InitializeComponent();
        _volumeRampTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
        _volumeRampTimer.Tick += VolumeRampTimer_Tick;
        _scrollToCursorTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(120) };
        _scrollToCursorTimer.Tick += ScrollToCursorTimer_Tick;
        _resizeRenderTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(260) };
        _resizeRenderTimer.Tick += ResizeRenderTimer_Tick;
        DataContext = this;
        RestoreLibraryFolder(settings);
        RestoreRecentFiles(settings);
        RestoreFavoriteFiles(settings);
        RebuildCurrentTabFiles();
        RestorePlaybackSettings(settings);
        ApplyPlaybackSettings();
        Loaded += (_, _) =>
        {
            ConfigureAlphaTabPlayer();
            RestoreLastFile();
        };
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

    private void ChooseLibraryFolder_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Выбрать папку библиотеки табулатур",
            Multiselect = false
        };

        if (!string.IsNullOrWhiteSpace(_libraryFolderPath) && Directory.Exists(_libraryFolderPath))
        {
            dialog.InitialDirectory = _libraryFolderPath;
        }

        if (dialog.ShowDialog() != true)
            return;

        SetLibraryFolder(dialog.FolderName);
    }

    private void RefreshLibrary_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_libraryFolderPath))
        {
            StatusText.Text = "Папка библиотеки не выбрана";
            return;
        }

        ScanLibraryFolder();
        StatusText.Text = LibraryTabFiles.Count > 0
            ? $"Библиотека обновлена: {LibraryTabFiles.Count} файлов"
            : "В папке библиотеки табы не найдены";
    }

    private void LoadTab(string path, int? selectedTrackIndex = null, bool showError = true)
    {
        try
        {
            double restoreTick = path.Equals(_settings?.LastTabFilePath, StringComparison.OrdinalIgnoreCase)
                ? Math.Max(0d, _settings.LastTabTickPosition)
                : 0d;

            byte[] data = File.ReadAllBytes(path);
            _score = ScoreLoader.LoadScoreFromBytes(data, new Settings());
            _loadedFilePath = path;
            _lastKnownTickPosition = restoreTick;
            ConfigureAlphaTabPlayer();

            TrackCombo.ItemsSource = _score.Tracks
                .Select((track, index) => new TabTrackItem(track, index + 1))
                .ToList();

            if (_score.Tracks.Count > 0)
            {
                int index = Math.Clamp(selectedTrackIndex ?? 0, 0, _score.Tracks.Count - 1);
                TrackCombo.SelectedIndex = index;
            }
            else
            {
                TrackCombo.SelectedIndex = -1;
            }

            ApplyPlaybackSettings();
            ApplyTrackPlaybackMode();
            QueueCursorRestore(restoreTick);
            AddRecentTabFile(path);
            UpdateFavoriteToggleState();

            StatusText.Text = $"{Path.GetFileName(path)}";
        }
        catch (Exception ex)
        {
            AppLogger.Warning($"Failed to load tab file '{path}'.", ex);
            TracksToDisplay.Clear();
            TrackCombo.ItemsSource = null;
            StatusText.Text = "Файл не загружен";

            if (showError)
            {
                MessageBox.Show(
                    $"Не удалось открыть табулатуру.\n\n{ex.Message}",
                    "GuitarToolkit",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
    }

    private void FileSourceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        RebuildCurrentTabFiles();
    }

    private void FileChoiceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isUpdatingCurrentFiles)
            return;

        if (FileChoiceCombo.SelectedItem is not TabFileItem item)
            return;

        if (!File.Exists(item.Path))
        {
            RemoveMissingTabFile(item.Path);
            StatusText.Text = "Файл из выбранного списка не найден";
            return;
        }

        LoadTab(item.Path);
    }

    private void FavoriteToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingFavoriteToggle)
            return;

        if (string.IsNullOrWhiteSpace(_loadedFilePath))
        {
            UpdateFavoriteToggleState();
            return;
        }

        if (FavoriteToggle.IsChecked == true)
        {
            AddFavoriteTabFile(_loadedFilePath);
            StatusText.Text = $"Добавлено в избранное: {Path.GetFileName(_loadedFilePath)}";
        }
        else
        {
            RemoveFavoriteTabFile(_loadedFilePath);
            StatusText.Text = $"Удалено из избранного: {Path.GetFileName(_loadedFilePath)}";
        }

        UpdateFavoriteToggleState();
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

    public bool HandleShortcut(KeyEventArgs e)
    {
        if (Keyboard.FocusedElement is TextBox)
            return false;

        if (e.Key == Key.Space)
        {
            TogglePlayback();
            e.Handled = true;
            return true;
        }

        if (e.Key == Key.Escape)
        {
            StopPlayback();
            e.Handled = true;
            return true;
        }

        return false;
    }

    private void PlayPauseToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (_isUpdatingPlayButton)
            return;

        if (PlayPauseToggle.IsChecked == true)
        {
            StartPlayback();
        }
        else
        {
            PausePlayback();
        }
    }

    private void Stop_Click(object sender, RoutedEventArgs e)
    {
        StopPlayback();
    }

    private void TogglePlayback()
    {
        if (_score == null)
            return;

        if (AlphaTab.Api?.PlayerState == PlayerState.Playing)
        {
            SetPlayButtonState(false);
            PausePlayback();
        }
        else
        {
            SetPlayButtonState(true);
            StartPlayback();
        }
    }

    private void StartPlayback()
    {
        if (_score == null)
        {
            SetPlayButtonState(false);
            return;
        }

        bool started = AlphaTab.Api?.Play() == true;
        if (!started)
        {
            SetPlayButtonState(false);
        }
        else
        {
            UpdatePlayButtonVisual(true);
        }
    }

    private void PausePlayback()
    {
        if (_score == null)
            return;

        AlphaTab.Api?.Pause();
        UpdatePlayButtonVisual(false);
    }

    private void StopPlayback()
    {
        if (_score == null)
            return;

        AlphaTab.Api?.Stop();
        _lastKnownTickPosition = 0d;
        SetPlayButtonState(false);
    }

    private void SetPlayButtonState(bool isPlaying)
    {
        if (PlayPauseToggle == null)
            return;

        _isUpdatingPlayButton = true;
        PlayPauseToggle.IsChecked = isPlaying;
        _isUpdatingPlayButton = false;
        UpdatePlayButtonVisual(isPlaying);
    }

    private void UpdatePlayButtonVisual(bool isPlaying)
    {
        if (PlayPauseToggle != null)
        {
            PlayPauseToggle.Content = isPlaying ? "Пауза" : "▶ Играть";
        }
    }

    private void TabScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        TabScrollViewer.ScrollToVerticalOffset(TabScrollViewer.VerticalOffset - e.Delta);
        e.Handled = true;
    }

    private void TabScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        ConfigureAlphaTabPlayer();

        if (_score == null)
            return;

        _pendingResizeRenderPasses = 3;
        _resizeRenderTimer.Stop();
        _resizeRenderTimer.Start();
    }

    private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (VolumeLabel != null)
        {
            VolumeLabel.Text = $"{Math.Round(e.NewValue)}%";
        }

        if (_isInitializingSettings)
            return;

        ApplyPlaybackSettings();
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
        FitAlphaTabToViewport();
        api.Settings.Display.LayoutMode = LayoutMode.Page;
        api.Settings.Display.BarsPerRow = -1d;
        api.Settings.Display.JustifyLastSystem = true;
        api.Settings.Display.StretchForce = 0.9d;
        api.Settings.Display.Scale = GetAdaptiveNotationScale();

        _tabScrollHandler ??= new TabScrollHandler(
            TabScrollViewer,
            () => AutoScrollCheckBox?.IsChecked == true,
            () => 0d);
        api.CustomScrollHandler = _tabScrollHandler;
        api.UpdateSettings();

        if (!_playbackEventsAttached)
        {
            api.PlayedBeatChanged.On(HandlePlayedBeatChanged);
            api.PlayerPositionChanged.On(HandlePlayerPositionChanged);
            api.PlayerStateChanged.On(HandlePlayerStateChanged);
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
        _lastKnownTickPosition = Math.Max(0d, e.CurrentTick);
        _tabScrollHandler?.SetCurrentPosition(e.CurrentTime, e.CurrentTick);
    }

    private void HandlePlayerStateChanged(PlayerStateChangedEventArgs e)
    {
        Dispatcher.BeginInvoke(() =>
        {
            SetPlayButtonState(e.State == PlayerState.Playing);
        }, DispatcherPriority.Background);
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

        double delay = 120d;
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

    private void ResizeRenderTimer_Tick(object? sender, EventArgs e)
    {
        var api = AlphaTab?.Api;
        if (api == null || _score == null || TracksToDisplay.Count == 0)
        {
            _resizeRenderTimer.Stop();
            return;
        }

        _pendingTickAfterRender = api.TickPosition;
        _pendingScrollAfterRender = AutoScrollCheckBox?.IsChecked == true;
        ConfigureAlphaTabPlayer();
        FitAlphaTabToViewport();
        AlphaTab?.InvalidateMeasure();
        AlphaTab?.UpdateLayout();
        AlphaTab?.RenderTracks();

        _pendingResizeRenderPasses--;
        if (_pendingResizeRenderPasses <= 0)
        {
            _resizeRenderTimer.Stop();
        }
    }

    private double GetAdaptiveNotationScale()
    {
        double width = TabScrollViewer?.ActualWidth ?? ActualWidth;

        if (width <= 0d)
            return 0.9d;

        if (width < 760d)
            return 0.7d;

        if (width < 980d)
            return 0.78d;

        if (width < 1250d)
            return 0.86d;

        return 0.95d;
    }

    private void FitAlphaTabToViewport()
    {
        if (AlphaTab == null || TabScrollViewer == null)
            return;

        double width = TabScrollViewer.ViewportWidth > 0d
            ? TabScrollViewer.ViewportWidth
            : TabScrollViewer.ActualWidth;

        if (width > 0d)
        {
            AlphaTab.Width = Math.Max(320d, width - 18d);
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

    public void SaveTo(UserSettings settings)
    {
        settings.LastTabFilePath = _loadedFilePath ?? "";
        settings.TabLibraryFolderPath = _libraryFolderPath ?? "";
        settings.RecentTabFilePaths = RecentTabFiles
            .Select(item => item.Path)
            .ToList();
        settings.FavoriteTabFilePaths = FavoriteTabFiles
            .Select(item => item.Path)
            .ToList();
        settings.LastTabTrackIndex = Math.Max(0, TrackCombo?.SelectedIndex ?? 0);
        settings.LastTabTickPosition = GetCurrentTickPosition();
        settings.TabVolumePercent = Math.Clamp(VolumeSlider?.Value ?? 35d, 0d, 100d);
        settings.TabSpeedPercent = GetPlaybackSpeedPercent();
        settings.TabAutoScroll = AutoScrollCheckBox?.IsChecked == true;
        settings.TabSyncOffsetMilliseconds = 0d;
        settings.TabSoloSelectedTrack = SoloToggle?.IsChecked == true;
        settings.TabMuteSelectedTrack = MuteToggle?.IsChecked == true;
    }

    private void RestorePlaybackSettings(UserSettings? settings)
    {
        if (settings == null)
            return;

        _isInitializingSettings = true;

        VolumeSlider.Value = Math.Clamp(settings.TabVolumePercent, 0d, 100d);
        VolumeLabel.Text = $"{Math.Round(VolumeSlider.Value)}%";
        SetPlaybackSpeedPercent(settings.TabSpeedPercent);
        AutoScrollCheckBox.IsChecked = settings.TabAutoScroll;
        SoloToggle.IsChecked = settings.TabSoloSelectedTrack;
        MuteToggle.IsChecked = !settings.TabSoloSelectedTrack && settings.TabMuteSelectedTrack;

        _isInitializingSettings = false;
    }

    private void RestoreLastFile()
    {
        if (_restoredLastFile)
            return;

        _restoredLastFile = true;

        if (_settings == null || string.IsNullOrWhiteSpace(_settings.LastTabFilePath))
            return;

        if (!File.Exists(_settings.LastTabFilePath))
        {
            StatusText.Text = "Последний файл табулатуры не найден";
            return;
        }

        LoadTab(_settings.LastTabFilePath, _settings.LastTabTrackIndex, showError: false);
    }

    private void RestoreLibraryFolder(UserSettings? settings)
    {
        if (settings == null || string.IsNullOrWhiteSpace(settings.TabLibraryFolderPath))
            return;

        SetLibraryFolder(settings.TabLibraryFolderPath, updateStatus: false);
    }

    private void SetLibraryFolder(string folderPath, bool updateStatus = true)
    {
        _libraryFolderPath = folderPath;
        ScanLibraryFolder();

        if (updateStatus)
        {
            StatusText.Text = LibraryTabFiles.Count > 0
                ? $"Библиотека: {LibraryTabFiles.Count} файлов"
                : "В папке библиотеки табы не найдены";
        }
    }

    private void ScanLibraryFolder()
    {
        LibraryTabFiles.Clear();

        if (string.IsNullOrWhiteSpace(_libraryFolderPath) || !Directory.Exists(_libraryFolderPath))
        {
            return;
        }

        try
        {
            foreach (string path in Directory.EnumerateFiles(_libraryFolderPath, "*.*", SearchOption.AllDirectories)
                         .Where(IsSupportedTabFile)
                         .OrderBy(Path.GetFileName, StringComparer.CurrentCultureIgnoreCase)
                         .Take(500))
            {
                LibraryTabFiles.Add(new TabFileItem(path, _libraryFolderPath));
            }
        }
        catch (Exception ex)
        {
            AppLogger.Warning($"Failed to scan tab library folder '{_libraryFolderPath}'.", ex);
            StatusText.Text = "Не удалось прочитать папку библиотеки";
        }
        finally
        {
            RebuildCurrentTabFiles();
        }
    }

    private static bool IsSupportedTabFile(string path)
    {
        string extension = Path.GetExtension(path);
        return SupportedTabExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    private void RestoreRecentFiles(UserSettings? settings)
    {
        if (settings == null)
            return;

        RecentTabFiles.Clear();

        foreach (string path in settings.RecentTabFilePaths
                     .Where(path => !string.IsNullOrWhiteSpace(path))
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .Take(MaxRecentTabFiles))
        {
            RecentTabFiles.Add(new RecentTabFileItem(path));
        }

        RebuildCurrentTabFiles();
    }

    private void RestoreFavoriteFiles(UserSettings? settings)
    {
        if (settings == null)
            return;

        FavoriteTabFiles.Clear();

        foreach (string path in settings.FavoriteTabFilePaths
                     .Where(path => !string.IsNullOrWhiteSpace(path))
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .Take(MaxFavoriteTabFiles))
        {
            FavoriteTabFiles.Add(new TabFileItem(path));
        }

        RebuildCurrentTabFiles();
    }

    private void RebuildCurrentTabFiles()
    {
        if (CurrentTabFiles == null)
            return;

        _isUpdatingCurrentFiles = true;
        CurrentTabFiles.Clear();

        IEnumerable<TabFileItem> source = GetSelectedFileSourceIndex() switch
        {
            1 => RecentTabFiles,
            2 => FavoriteTabFiles,
            _ => LibraryTabFiles
        };

        foreach (var item in source)
        {
            CurrentTabFiles.Add(item);
        }

        if (FileChoiceCombo != null)
        {
            FileChoiceCombo.SelectedIndex = -1;
        }
        _isUpdatingCurrentFiles = false;
    }

    private int GetSelectedFileSourceIndex()
    {
        return FileSourceCombo?.SelectedIndex >= 0
            ? FileSourceCombo.SelectedIndex
            : 0;
    }

    private void RemoveMissingTabFile(string path)
    {
        RemoveRecentTabFile(path);
        RemoveFavoriteTabFile(path);

        if (_libraryFolderPath != null && path.StartsWith(_libraryFolderPath, StringComparison.OrdinalIgnoreCase))
        {
            ScanLibraryFolder();
        }

        RebuildCurrentTabFiles();
    }

    private void AddRecentTabFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        var existing = RecentTabFiles
            .FirstOrDefault(item => item.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            RecentTabFiles.Remove(existing);
        }

        RecentTabFiles.Insert(0, new RecentTabFileItem(path));

        while (RecentTabFiles.Count > MaxRecentTabFiles)
        {
            RecentTabFiles.RemoveAt(RecentTabFiles.Count - 1);
        }

        RebuildCurrentTabFiles();
    }

    private void AddFavoriteTabFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        var existing = FavoriteTabFiles
            .FirstOrDefault(item => item.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            FavoriteTabFiles.Remove(existing);
        }

        FavoriteTabFiles.Insert(0, new TabFileItem(path));

        while (FavoriteTabFiles.Count > MaxFavoriteTabFiles)
        {
            FavoriteTabFiles.RemoveAt(FavoriteTabFiles.Count - 1);
        }

        RebuildCurrentTabFiles();
    }

    private void RemoveFavoriteTabFile(string path)
    {
        var existing = FavoriteTabFiles
            .FirstOrDefault(item => item.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            FavoriteTabFiles.Remove(existing);
        }

        RebuildCurrentTabFiles();
    }

    private void UpdateFavoriteToggleState()
    {
        if (FavoriteToggle == null)
            return;

        bool isFavorite = !string.IsNullOrWhiteSpace(_loadedFilePath)
            && FavoriteTabFiles.Any(item => item.Path.Equals(_loadedFilePath, StringComparison.OrdinalIgnoreCase));

        _isUpdatingFavoriteToggle = true;
        FavoriteToggle.IsChecked = isFavorite;
        FavoriteToggle.Content = isFavorite ? "★" : "☆";
        FavoriteToggle.ToolTip = isFavorite
            ? "Удалить текущий файл из избранного"
            : "Добавить текущий файл в избранное";
        _isUpdatingFavoriteToggle = false;
    }

    private void RemoveRecentTabFile(string path)
    {
        var existing = RecentTabFiles
            .FirstOrDefault(item => item.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            RecentTabFiles.Remove(existing);
        }

        RebuildCurrentTabFiles();
    }

    private void QueueCursorRestore(double tickPosition)
    {
        if (tickPosition <= 0d)
            return;

        _pendingTickAfterRender = tickPosition;
        _pendingScrollAfterRender = AutoScrollCheckBox?.IsChecked == true;
    }

    private double GetCurrentTickPosition()
    {
        var api = AlphaTab?.Api;
        if (api != null)
        {
            return Math.Max(0d, api.TickPosition);
        }

        return Math.Max(0d, _lastKnownTickPosition);
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

    public sealed class RecentTabFileItem : TabFileItem
    {
        public RecentTabFileItem(string path)
            : base(path)
        {
        }
    }

    public class TabFileItem
    {
        public TabFileItem(string path, string? baseFolder = null)
        {
            Path = path;
            DisplayName = GetDisplayName(path, baseFolder);
        }

        public string Path { get; }
        public string DisplayName { get; }

        public override string ToString()
        {
            return DisplayName;
        }

        private static string GetDisplayName(string path, string? baseFolder)
        {
            if (string.IsNullOrWhiteSpace(baseFolder))
                return System.IO.Path.GetFileName(path);

            try
            {
                string relative = System.IO.Path.GetRelativePath(baseFolder, path);
                return relative == "."
                    ? System.IO.Path.GetFileName(path)
                    : relative;
            }
            catch
            {
                return System.IO.Path.GetFileName(path);
            }
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

            if (!force && (DateTime.UtcNow - _lastScrollUtc).TotalMilliseconds < 220)
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

                double cursorX = beatBounds.OnNotesX > 0d
                    ? beatBounds.OnNotesX
                    : bounds.X + bounds.W / 2d;
                double viewportLeft = _scrollViewer.HorizontalOffset;
                double viewportRight = viewportLeft + _scrollViewer.ViewportWidth;
                double viewportWidth = _scrollViewer.ViewportWidth;
                double comfortRight = viewportLeft + viewportWidth * 0.84d;

                if (force)
                {
                    double targetX = Math.Max(0d, cursorX - viewportWidth * 0.40d);
                    _scrollViewer.ScrollToHorizontalOffset(targetX);
                }
                else if (cursorX > comfortRight)
                {
                    double targetX = Math.Max(0d, cursorX - viewportWidth * 0.38d);
                    if (targetX > viewportLeft)
                    {
                        _scrollViewer.ScrollToHorizontalOffset(targetX);
                    }
                }
            }, DispatcherPriority.Background);
        }
    }
}
