using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GuitarToolkit.Core.Models;

namespace GuitarToolkit.UI;

public partial class FretboardView : UserControl
{
    private string _selectedRoot = "C";
    private int _selectedRootSemitone = 0;
    private ScaleDefinition _selectedScale;
    private bool _showIntervals = false; // false = ноты, true = интервалы

    private readonly List<Button> _rootButtons = new();

    // Цвета
    private static readonly Color RootColor = Color.FromRgb(166, 227, 161);    // зелёный
    private static readonly Color NoteColor = Color.FromRgb(203, 166, 247);    // голубой
    private static readonly Color TextDark = Color.FromRgb(30, 30, 46);
    private static readonly Color TextLight = Color.FromRgb(205, 214, 244);
    private static readonly Color FretColor = Color.FromRgb(90, 72, 110);
    private static readonly Color StringCol = Color.FromRgb(166, 173, 200);
    private static readonly Color ActiveBg = Color.FromRgb(203, 166, 247);
    private static readonly Color InactiveBg = Color.FromRgb(74, 56, 96);
    private static readonly Color MarkerColor = Color.FromRgb(45, 34, 64);

    // Настройки грифа
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

        Loaded += (s, e) => DrawFretboard();
        SizeChanged += (s, e) => DrawFretboard();
    }

    // ── Построение UI ────────────────────────────────────────

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
                Content = note,
                Width = 40, Height = 30,
                FontSize = 13, FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(InactiveBg),
                Foreground = new SolidColorBrush(TextLight),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(2),
                Tag = semitone
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

    // ── События ──────────────────────────────────────────────

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

    // ── Рисование грифа ──────────────────────────────────────

    private void DrawFretboard()
    {
        FretboardCanvas.Children.Clear();

        double canvasW = FretboardCanvas.ActualWidth;
        double canvasH = FretboardCanvas.ActualHeight;
        if (canvasW < 100 || canvasH < 50) return;

        double leftPad = 32;  // место для подписей струн
        double topPad = 10;
        double bottomPad = 25;
        double gridW = canvasW - leftPad - 10;
        double gridH = canvasH - topPad - bottomPad;

        double fretW = gridW / FretCount;
        double stringSpacing = gridH / (StringCount - 1);

        // Инфо
        string notes = string.Join(" ",
            _selectedScale.Intervals.Select(i => ScaleLibrary.NoteNames[(_selectedRootSemitone + i) % 12]));
        InfoLabel.Text = $"{_selectedRoot} {_selectedScale.Name}: {notes}";

        // ── Порожек (нулевой лад) ────────────────────────────
        var nut = new Line
        {
            X1 = leftPad, Y1 = topPad,
            X2 = leftPad, Y2 = topPad + gridH,
            Stroke = new SolidColorBrush(TextLight),
            StrokeThickness = 4
        };
        FretboardCanvas.Children.Add(nut);

        // ── Лады (вертикальные линии) ────────────────────────
        for (int f = 1; f <= FretCount; f++)
        {
            double x = leftPad + f * fretW;
            var line = new Line
            {
                X1 = x, Y1 = topPad,
                X2 = x, Y2 = topPad + gridH,
                Stroke = new SolidColorBrush(FretColor),
                StrokeThickness = 1.2
            };
            FretboardCanvas.Children.Add(line);

            // Номер лада внизу
            var label = new TextBlock
            {
                Text = f.ToString(),
                FontSize = 10,
                Foreground = new SolidColorBrush(FretColor),
                TextAlignment = TextAlignment.Center,
                Width = fretW
            };
            Canvas.SetLeft(label, leftPad + (f - 1) * fretW);
            Canvas.SetTop(label, topPad + gridH + 4);
            FretboardCanvas.Children.Add(label);
        }

        // ── Маркеры (точки на ладах 3, 5, 7, 9, 12, 15) ────
        foreach (int f in DotFrets)
        {
            if (f > FretCount) break;
            double cx = leftPad + (f - 0.5) * fretW;
            bool isDouble = DoubleDotFrets.Contains(f);

            if (isDouble)
            {
                DrawDot(cx, topPad + gridH * 0.28, 5, MarkerColor);
                DrawDot(cx, topPad + gridH * 0.72, 5, MarkerColor);
            }
            else
            {
                DrawDot(cx, topPad + gridH * 0.5, 5, MarkerColor);
            }
        }

        // ── Струны (горизонтальные линии) ────────────────────
        for (int s = 0; s < StringCount; s++)
        {
            double y = topPad + s * stringSpacing;

            // Подпись слева
            var label = new TextBlock
            {
                Text = StringLabels[s],
                FontSize = 12,
                Foreground = new SolidColorBrush(StringCol),
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(label, 8);
            Canvas.SetTop(label, y - 8);
            FretboardCanvas.Children.Add(label);

            // Линия струны
            var line = new Line
            {
                X1 = leftPad, Y1 = y,
                X2 = leftPad + gridW, Y2 = y,
                Stroke = new SolidColorBrush(StringCol),
                StrokeThickness = 2.0 - s * 0.2,
                Opacity = 0.6
            };
            FretboardCanvas.Children.Add(line);
        }

        // ── Ноты гаммы ──────────────────────────────────────
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
                double cx = f == 0
                    ? leftPad - 1
                    : leftPad + (f - 0.5) * fretW;

                double dotSize = Math.Min(fretW * 0.7, stringSpacing * 0.7);
                dotSize = Math.Clamp(dotSize, 16, 28);

                // Кружок
                var dot = new Ellipse
                {
                    Width = dotSize, Height = dotSize,
                    Fill = new SolidColorBrush(isRoot ? RootColor : NoteColor)
                };
                Canvas.SetLeft(dot, cx - dotSize / 2);
                Canvas.SetTop(dot, y - dotSize / 2);
                FretboardCanvas.Children.Add(dot);

                // Текст
                string text = _showIntervals
                    ? ScaleLibrary.GetInterval(note, _selectedRootSemitone)
                    : ScaleLibrary.NoteNames[note];

                var tb = new TextBlock
                {
                    Text = text,
                    FontSize = dotSize * 0.4,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(TextDark),
                    TextAlignment = TextAlignment.Center,
                    Width = dotSize,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Canvas.SetLeft(tb, cx - dotSize / 2);
                Canvas.SetTop(tb, y - dotSize * 0.25);
                FretboardCanvas.Children.Add(tb);
            }
        }
    }

    private void DrawDot(double cx, double cy, double radius, Color color)
    {
        var dot = new Ellipse
        {
            Width = radius * 2,
            Height = radius * 2,
            Fill = new SolidColorBrush(color)
        };
        Canvas.SetLeft(dot, cx - radius);
        Canvas.SetTop(dot, cy - radius);
        FretboardCanvas.Children.Add(dot);
    }
}
