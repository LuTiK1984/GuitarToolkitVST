using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using AlphaTab;
using AlphaTab.Importer;
using AlphaTab.Model;
using Microsoft.Win32;

namespace GuitarToolkit.UI;

public partial class TabPlayerView : UserControl
{
    private Score? _score;

    public ObservableCollection<Track> TracksToDisplay { get; } = new();

    public TabPlayerView()
    {
        InitializeComponent();
        DataContext = this;
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

            TrackCombo.ItemsSource = _score.Tracks
                .Select((track, index) => new TabTrackItem(track, index + 1))
                .ToList();
            TrackCombo.SelectedIndex = _score.Tracks.Count > 0 ? 0 : -1;

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
            TracksToDisplay.Add(item.Track);
            AlphaTab.RenderTracks();
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
}
