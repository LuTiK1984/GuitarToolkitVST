using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GuitarToolkit.Core.DSP;
using GuitarToolkit.Core.Models;

namespace GuitarToolkit.UI;

public partial class FretboardView : UserControl
{
    private IAudioPlayback? _audio;
    private string _selectedRoot = "C";
    private int _selectedRootSemitone = 0;
    private ScaleDefinition _selectedScale;
    private bool _showIntervals = false;

    private readonly List<Button> _rootButtons = new();

    private static readonly Color RootColor = Color.FromRgb(166, 227, 161);
    private static readonly Color NoteColor = Color.FromRgb(203, 166, 247);
    private static readonly Color TextDark = Color.FromRgb(26, 21, 37);
    private static readonly Color TextLight = Color.FromRgb(205, 214, 244);
    private static readonly Color FretColor = Color.FromRgb(90, 72, 110);
    private static readonly Color StringCol = Color.FromRgb(166, 173, 200);
    private static readonly Color ActiveBg = Color.FromRgb(203, 166, 247);
    private static readonly Color InactiveBg = Color.FromRgb(74, 56, 96);
    private static readonly Color MarkerColor = Color.FromRgb(45, 34, 64);

    private const int FretCount = 15;
    private const int StringCount = 6;
    private static readonly int[] DotFrets = { 3, 5, 7, 9, 12, 15 };
    private static readonly int[] DoubleDotFrets = { 12 };
    private static readonly string[] StringLabels = { "E", "A", "D", "G", "B", "e" };

    public FretboardView()
    {
        InitializeComponent();
        _selectedScale = ScaleLibrary.All[0];

        BuildRootButtons();
        BuildScaleBox();
    }

    public void Initialize(IAudioPlayback audio)
    {
        _audio = audio;
    }

    private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        DrawFretboard();
    }

    private void BuildRootButtons()
    {
        RootSelector.Items.Clear();
        _rootButtons.Clear();

        for (int i = 0; i < ScaleLibrary.NoteNames.Length; i++)
        {
            string note = ScaleLibrary.NoteNames[i];
            int semitone = i;

            var btn = new Button
            {
                Content = note, Width = 40, Height = 30,
                FontSize = 13, FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(InactiveBg),
                Foreground = new SolidColorBrush(TextLight),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(2), Tag = semitone,
                ToolTip = $"Тоника: {note}"
            };
            btn.Click += (s, e) =>
            {
                _selectedRoot = note;
                _selectedRootSemitone = semitone;
                HighlightRootButtons();
                DrawFretboard();
            };
            _rootButtons.Add(btn);
            RootSelector.Items.Add(btn);
        }
        HighlightRootButtons();
    }

    private void BuildScaleBox()
    {
        ScaleBox.Items.Clear();
        foreach (var scale in ScaleLibrary.All)
            ScaleBox.Items.Add(scale.Name);
        ScaleBox.SelectedIndex = 0;
    }

    private void HighlightRootButtons()
    {
        foreach (var btn in _rootButtons)
        {
            bool active = btn.Content.ToString() == _selectedRoot;
            btn.Background = new SolidColorBrush(active ? ActiveBg : InactiveBg);
            btn.Foreground = new SolidColorBrush(active ? TextDark : TextLight);
        }
    }

    private void ScaleBox_SelectionChanged(object s, SelectionChangedEventArgs e)
    {
        if (ScaleBox.SelectedIndex >= 0 && ScaleBox.SelectedIndex < ScaleLibrary.All.Count)
        {
            _selectedScale = ScaleLibrary.All[ScaleBox.SelectedIndex];
            DrawFretboard();
        }
    }

    private void ModeButton_Click(object s, RoutedEventArgs e)
    {
        _showIntervals = !_showIntervals;
        ModeButton.Content = _showIntervals ? "Интервалы" : "Ноты";
        DrawFretboard();
    }

    // ── Воспроизведение гаммы ────────────────────────────────

    private void PlayScale_Click(object s, RoutedEventArgs e)
    {
        if (_audio == null) return;

        int baseMidi = 60 + _selectedRootSemitone;
        if (baseMidi > 72) baseMidi -= 12;

        int sr = _audio.SampleRate;
        var allSamples = new List<float>();

        foreach (int interval in _selectedScale.Intervals.Append(12))
        {
            float freq = 440f * MathF.Pow(2f, (baseMidi + interval - 69) / 12f);
            float[] note = NoteSynth.GenerateNote(freq, sr, duration: 0.35f, volume: 0.25f);
            allSamples.AddRange(note);
            allSamples.AddRange(new float[(int)(sr * 0.04f)]);
        }

        _audio.PlaySamples(allSamples.ToArray());
    }

    // ── Рисование ────────────────────────────────────────────

    private void DrawFretboard()
    {
        FretboardCanvas.Children.Clear();

        double canvasW = FretboardCanvas.ActualWidth;
        double canvasH = FretboardCanvas.ActualHeight;
        if (canvasW < 100 || canvasH < 50) return;

        double leftPad = 32;
        double topPad = 10;
        double bottomPad = 25;
        double gridW = canvasW - leftPad - 10;
        double gridH = canvasH - topPad - bottomPad;

        double fretW = gridW / FretCount;
        double stringSpacing = gridH / (StringCount - 1);

        string notes = string.Join(" ",
            _selectedScale.Intervals.Select(i => ScaleLibrary.NoteNames[(_selectedRootSemitone + i) % 12]));
        InfoLabel.Text = $"{_selectedRoot} {_selectedScale.Name}: {notes}";

        // Порожек
        FretboardCanvas.Children.Add(new Line
        {
            X1 = leftPad, Y1 = topPad, X2 = leftPad, Y2 = topPad + gridH,
            Stroke = new SolidColorBrush(TextLight), StrokeThickness = 4
        });

        // Лады + номера
        for (int f = 1; f <= FretCount; f++)
        {
            double x = leftPad + f * fretW;
            FretboardCanvas.Children.Add(new Line
            {
                X1 = x, Y1 = topPad, X2 = x, Y2 = topPad + gridH,
                Stroke = new SolidColorBrush(FretColor), StrokeThickness = 1.2
            });

            var label = new TextBlock
            {
                Text = f.ToString(), FontSize = 10,
                Foreground = new SolidColorBrush(FretColor),
                TextAlignment = TextAlignment.Center, Width = fretW
            };
            Canvas.SetLeft(label, leftPad + (f - 1) * fretW);
            Canvas.SetTop(label, topPad + gridH + 4);
            FretboardCanvas.Children.Add(label);
        }

        // Маркеры
        foreach (int f in DotFrets)
        {
            if (f > FretCount) break;
            double cx = leftPad + (f - 0.5) * fretW;
            bool isDouble = DoubleDotFrets.Contains(f);

            if (isDouble)
            {
                DrawMarker(cx, topPad + gridH * 0.28, 5);
                DrawMarker(cx, topPad + gridH * 0.72, 5);
            }
            else
            {
                DrawMarker(cx, topPad + gridH * 0.5, 5);
            }
        }

        // Струны + подписи
        for (int s = 0; s < StringCount; s++)
        {
            double y = topPad + s * stringSpacing;

            var label = new TextBlock
            {
                Text = StringLabels[s], FontSize = 12,
                Foreground = new SolidColorBrush(StringCol),
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(label, 8);
            Canvas.SetTop(label, y - 8);
            FretboardCanvas.Children.Add(label);

            FretboardCanvas.Children.Add(new Line
            {
                X1 = leftPad, Y1 = y, X2 = leftPad + gridW, Y2 = y,
                Stroke = new SolidColorBrush(StringCol),
                StrokeThickness = 2.0 - s * 0.2, Opacity = 0.6
            });
        }

        // Ноты гаммы
        for (int s = 0; s < StringCount; s++)
        {
            int openNote = ScaleLibrary.StandardTuning[s];
            double y = topPad + s * stringSpacing;

            for (int f = 0; f <= FretCount; f++)
            {
                int note = (openNote + f) % 12;
                if (!ScaleLibrary.IsInScale(note, _selectedRootSemitone, _selectedScale))
                    continue;

                bool isRoot = note == _selectedRootSemitone;
                double cx = f == 0 ? leftPad - 1 : leftPad + (f - 0.5) * fretW;
                double dotSize = Math.Clamp(Math.Min(fretW * 0.7, stringSpacing * 0.7), 16, 28);

                var dot = new Ellipse
                {
                    Width = dotSize, Height = dotSize,
                    Fill = new SolidColorBrush(isRoot ? RootColor : NoteColor)
                };
                Canvas.SetLeft(dot, cx - dotSize / 2);
                Canvas.SetTop(dot, y - dotSize / 2);
                FretboardCanvas.Children.Add(dot);

                string text = _showIntervals
                    ? ScaleLibrary.GetInterval(note, _selectedRootSemitone)
                    : ScaleLibrary.NoteNames[note];

                var tb = new TextBlock
                {
                    Text = text, FontSize = dotSize * 0.4,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(TextDark),
                    TextAlignment = TextAlignment.Center,
                    Width = dotSize
                };
                Canvas.SetLeft(tb, cx - dotSize / 2);
                Canvas.SetTop(tb, y - dotSize * 0.25);
                FretboardCanvas.Children.Add(tb);
            }
        }
    }

    private void DrawMarker(double cx, double cy, double radius)
    {
        var dot = new Ellipse
        {
            Width = radius * 2, Height = radius * 2,
            Fill = new SolidColorBrush(MarkerColor)
        };
        Canvas.SetLeft(dot, cx - radius);
        Canvas.SetTop(dot, cy - radius);
        FretboardCanvas.Children.Add(dot);
    }
}
