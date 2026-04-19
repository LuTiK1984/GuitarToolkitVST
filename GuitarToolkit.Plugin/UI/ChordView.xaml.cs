using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GuitarToolkit.Core.Models;
using GuitarToolkit.Core.DSP;

namespace GuitarToolkit.Plugin.UI;

public partial class ChordView : UserControl
{
    private GuitarToolkitPlugin? _plugin;
    private string _selectedRoot = "C";
    private string _selectedType = "Major";
    private ChordDefinition? _currentChord;

    // Кнопки для подсветки выбора
    private readonly List<Button> _rootButtons = new();
    private readonly List<Button> _typeButtons = new();

    // Цвета
    private static readonly Color ActiveBg = Color.FromRgb(203, 166, 247);    // #CBA6F7
    private static readonly Color InactiveBg = Color.FromRgb(74, 56, 96);     // #4A3860
    private static readonly Color TextLight = Color.FromRgb(205, 214, 244);   // #CDD6F4
    private static readonly Color TextDark = Color.FromRgb(30, 30, 46);       // #1A1525
    private static readonly Color StringColor = Color.FromRgb(166, 173, 200); // #A6ADC8
    private static readonly Color DotColor = Color.FromRgb(203, 166, 247);
    private static readonly Color MutedColor = Color.FromRgb(243, 139, 168);  // #F38BA8

    // Отображение типов
    private static readonly Dictionary<string, string> TypeLabels = new()
    {
        { "Major", "Мажор" }, { "m", "Минор" }, { "7", "Доминант 7" },
        { "maj7", "Мажор 7" }, { "m7", "Минор 7" }, { "sus2", "Sus2" },
        { "sus4", "Sus4" }, { "dim", "Уменьш." }, { "aug", "Увелич." }
    };

    public ChordView()
    {
        InitializeComponent();
    }

    public void Initialize(GuitarToolkitPlugin plugin)
    {
        _plugin = plugin;
        BuildRootButtons();
        BuildTypeButtons();
        UpdateChord();
    }

    // ── Построение кнопок выбора ─────────────────────────────

    private void BuildRootButtons()
    {
        RootSelector.Items.Clear();
        _rootButtons.Clear();

        foreach (string root in ChordLibrary.AllRoots)
        {
            var btn = MakeButton(root, 48, 38);
            btn.Click += (s, e) =>
            {
                _selectedRoot = root;
                HighlightButtons(_rootButtons, root);
                UpdateChord();
            };
            _rootButtons.Add(btn);
            RootSelector.Items.Add(btn);
        }

        HighlightButtons(_rootButtons, _selectedRoot);
    }

    private void BuildTypeButtons()
    {
        TypeSelector.Items.Clear();
        _typeButtons.Clear();

        foreach (string type in ChordLibrary.AllTypes)
        {
            string label = TypeLabels.GetValueOrDefault(type, type);
            var btn = MakeButton(label, 0, 38); // auto width
            btn.Tag = type;
            btn.Padding = new Thickness(14, 0, 14, 0);
            btn.Click += (s, e) =>
            {
                _selectedType = type;
                HighlightTypeButtons();
                UpdateChord();
            };
            _typeButtons.Add(btn);
            TypeSelector.Items.Add(btn);
        }

        HighlightTypeButtons();
    }

    private Button MakeButton(string text, double width, double height)
    {
        var btn = new Button
        {
            Content = text,
            Height = height,
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            Background = new SolidColorBrush(InactiveBg),
            Foreground = new SolidColorBrush(TextLight),
            BorderThickness = new Thickness(0),
            Cursor = System.Windows.Input.Cursors.Hand,
            Margin = new Thickness(3)
        };
        if (width > 0) btn.Width = width;
        return btn;
    }

    private void HighlightButtons(List<Button> buttons, string selectedText)
    {
        foreach (var btn in buttons)
        {
            bool active = btn.Content.ToString() == selectedText;
            btn.Background = new SolidColorBrush(active ? ActiveBg : InactiveBg);
            btn.Foreground = new SolidColorBrush(active ? TextDark : TextLight);
        }
    }

    private void HighlightTypeButtons()
    {
        foreach (var btn in _typeButtons)
        {
            bool active = btn.Tag?.ToString() == _selectedType;
            btn.Background = new SolidColorBrush(active ? ActiveBg : InactiveBg);
            btn.Foreground = new SolidColorBrush(active ? TextDark : TextLight);
        }
    }

    // ── Обновление аккорда ───────────────────────────────────

    private void UpdateChord()
    {
        _currentChord = ChordLibrary.Get(_selectedRoot, _selectedType);

        if (_currentChord != null)
        {
            ChordNameLabel.Text = _currentChord.DisplayName;
            string fretsStr = string.Join(" ",
                _currentChord.Frets.Select(f => f < 0 ? "x" : f.ToString()));
            InfoLabel.Text = $"Аппликатура: {fretsStr}\nПозиция: {_currentChord.BaseFret} лад";
            DrawDiagram(_currentChord);
        }
        else
        {
            ChordNameLabel.Text = $"{_selectedRoot}?";
            InfoLabel.Text = "Аккорд не найден в библиотеке";
            DiagramCanvas.Children.Clear();
        }
    }

    // ── Рисование диаграммы ──────────────────────────────────

    private void DrawDiagram(ChordDefinition chord)
    {
        DiagramCanvas.Children.Clear();

        double canvasW = DiagramCanvas.Width;
        double canvasH = DiagramCanvas.Height;

        // Размеры сетки
        double leftPad = 30;
        double topPad = 30;
        double gridW = canvasW - leftPad - 20;
        double gridH = canvasH - topPad - 20;
        int fretCount = 5;

        double stringSpacing = gridW / 5.0;
        double fretSpacing = gridH / fretCount;

        // ── Порожек или номер лада ───────────────────────────
        if (chord.BaseFret <= 1)
        {
            // Жирная линия (порожек)
            var nut = new Line
            {
                X1 = leftPad, Y1 = topPad,
                X2 = leftPad + gridW, Y2 = topPad,
                Stroke = new SolidColorBrush(TextLight),
                StrokeThickness = 5,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };
            DiagramCanvas.Children.Add(nut);
        }
        else
        {
            // Номер лада слева
            var label = new TextBlock
            {
                Text = chord.BaseFret.ToString(),
                FontSize = 14,
                Foreground = new SolidColorBrush(TextLight),
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(label, 4);
            Canvas.SetTop(label, topPad + fretSpacing * 0.3);
            DiagramCanvas.Children.Add(label);
        }

        // ── Лады (горизонтальные линии) ──────────────────────
        for (int f = 0; f <= fretCount; f++)
        {
            double y = topPad + f * fretSpacing;
            var line = new Line
            {
                X1 = leftPad, Y1 = y,
                X2 = leftPad + gridW, Y2 = y,
                Stroke = new SolidColorBrush(Color.FromRgb(90, 72, 110)),
                StrokeThickness = f == 0 && chord.BaseFret > 1 ? 2 : 1
            };
            DiagramCanvas.Children.Add(line);
        }

        // ── Струны (вертикальные линии) ──────────────────────
        for (int s = 0; s < 6; s++)
        {
            double x = leftPad + s * stringSpacing;
            var line = new Line
            {
                X1 = x, Y1 = topPad,
                X2 = x, Y2 = topPad + gridH,
                Stroke = new SolidColorBrush(StringColor),
                StrokeThickness = 1.5 - s * 0.1 // толстые басовые
            };
            DiagramCanvas.Children.Add(line);
        }

        // ── Баррэ ────────────────────────────────────────────
        if (chord.BaseFret > 1)
        {
            int barreFret = chord.Frets.Where(f => f >= 0).Min();
            int barreRelative = barreFret - chord.BaseFret + 1;

            int firstStr = -1, lastStr = -1;
            for (int s = 0; s < 6; s++)
            {
                if (chord.Frets[s] == barreFret)
                {
                    if (firstStr < 0) firstStr = s;
                    lastStr = s;
                }
            }

            if (firstStr >= 0 && lastStr > firstStr)
            {
                double y = topPad + (barreRelative - 0.5) * fretSpacing;
                double x1 = leftPad + firstStr * stringSpacing;
                double x2 = leftPad + lastStr * stringSpacing;

                var barre = new Line
                {
                    X1 = x1, Y1 = y, X2 = x2, Y2 = y,
                    Stroke = new SolidColorBrush(DotColor),
                    StrokeThickness = 8,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    Opacity = 0.8
                };
                DiagramCanvas.Children.Add(barre);
            }
        }

        // ── Точки пальцев + маркеры X/O ─────────────────────
        for (int s = 0; s < 6; s++)
        {
            double x = leftPad + s * stringSpacing;
            int fret = chord.Frets[s];

            if (fret < 0)
            {
                // X — заглушена
                var mark = new TextBlock
                {
                    Text = "✕",
                    FontSize = 16,
                    Foreground = new SolidColorBrush(MutedColor),
                    FontWeight = FontWeights.Bold
                };
                Canvas.SetLeft(mark, x - 7);
                Canvas.SetTop(mark, topPad - 25);
                DiagramCanvas.Children.Add(mark);
            }
            else if (fret == 0)
            {
                // O — открытая
                var circle = new Ellipse
                {
                    Width = 14, Height = 14,
                    Stroke = new SolidColorBrush(TextLight),
                    StrokeThickness = 2,
                    Fill = Brushes.Transparent
                };
                Canvas.SetLeft(circle, x - 7);
                Canvas.SetTop(circle, topPad - 22);
                DiagramCanvas.Children.Add(circle);
            }
            else
            {
                // Точка на ладу
                int relativeFret = fret - chord.BaseFret + 1;
                if (relativeFret < 1) relativeFret = 1;
                if (relativeFret > fretCount) relativeFret = fretCount;

                double y = topPad + (relativeFret - 0.5) * fretSpacing;
                double dotSize = 18;

                var dot = new Ellipse
                {
                    Width = dotSize, Height = dotSize,
                    Fill = new SolidColorBrush(DotColor)
                };
                Canvas.SetLeft(dot, x - dotSize / 2);
                Canvas.SetTop(dot, y - dotSize / 2);
                DiagramCanvas.Children.Add(dot);
            }
        }

        // ── Подписи струн внизу ──────────────────────────────
        string[] stringNames = { "E", "A", "D", "G", "B", "e" };
        for (int s = 0; s < 6; s++)
        {
            double x = leftPad + s * stringSpacing;
            var label = new TextBlock
            {
                Text = stringNames[s],
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(124, 111, 150))
            };
            Canvas.SetLeft(label, x - 4);
            Canvas.SetTop(label, topPad + gridH + 4);
            DiagramCanvas.Children.Add(label);
        }
    }

    // ── Воспроизведение ──────────────────────────────────────

    private void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentChord == null || _plugin == null) return;

        int sr = 44100;
        try { sr = (int)_plugin.Host.SampleRate; } catch { }
        if (sr <= 0) sr = 44100;

        float[] samples = ChordPlayer.Synthesize(_currentChord, sr);
        _plugin.PlayChordSamples(samples);
    }
}
