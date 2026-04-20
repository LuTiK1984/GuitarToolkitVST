using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GuitarToolkit.Core.Models;
using GuitarToolkit.Core.DSP;

namespace GuitarToolkit.UI;

public partial class ChordView : UserControl
{
    private IAudioPlayback? _audioHost;
    private string _selectedRoot = "C";
    private string _selectedType = "Major";
    private IReadOnlyList<ChordDefinition> _voicings = Array.Empty<ChordDefinition>();
    private int _currentVoicingIndex = 0;
    private ChordDefinition? _currentChord;

    private string _filter = "all"; // "all", "easy", "barre", "fav"

    private readonly List<Button> _rootButtons = new();
    private readonly List<Button> _typeButtons = new();

    private static readonly Color AccentColor = Color.FromRgb(203, 166, 247);
    private static readonly Color InactiveBg = Color.FromRgb(74, 56, 96);
    private static readonly Color TextLight = Color.FromRgb(205, 214, 244);
    private static readonly Color TextDark = Color.FromRgb(26, 21, 37);
    private static readonly Color StringColor = Color.FromRgb(166, 173, 200);
    private static readonly Color DotColor = Color.FromRgb(203, 166, 247);
    private static readonly Color MutedColor = Color.FromRgb(243, 139, 168);
    private static readonly Color FavColor = Color.FromRgb(249, 226, 175);

    private static readonly Dictionary<string, string> TypeLabels = new()
    {
        { "Major", "Мажор" }, { "m", "Минор" }, { "7", "Дом.7" },
        { "maj7", "Маж.7" }, { "m7", "Мин.7" }, { "sus2", "Sus2" },
        { "sus4", "Sus4" }, { "dim", "Умен." }, { "aug", "Увел." }
    };

    public ChordView()
    {
        InitializeComponent();
    }

    public void Initialize(IAudioPlayback audioHost)
    {
        _audioHost = audioHost;
        ChordLibrary.LoadFavorites();
        BuildRootButtons();
        BuildTypeButtons();
        UpdateChord();
    }

    // ── Фильтры ──────────────────────────────────────────────

    private void FilterAll_Click(object s, RoutedEventArgs e) => SetFilter("all");
    private void FilterEasy_Click(object s, RoutedEventArgs e) => SetFilter("easy");
    private void FilterBarre_Click(object s, RoutedEventArgs e) => SetFilter("barre");
    private void FilterFav_Click(object s, RoutedEventArgs e) => SetFilter("fav");

    private void SetFilter(string filter)
    {
        _filter = filter;
        FilterAllBtn.Background = new SolidColorBrush(filter == "all" ? AccentColor : InactiveBg);
        FilterAllBtn.Foreground = new SolidColorBrush(filter == "all" ? TextDark : TextLight);
        FilterEasyBtn.Background = new SolidColorBrush(filter == "easy" ? AccentColor : InactiveBg);
        FilterEasyBtn.Foreground = new SolidColorBrush(filter == "easy" ? TextDark : TextLight);
        FilterBarreBtn.Background = new SolidColorBrush(filter == "barre" ? AccentColor : InactiveBg);
        FilterBarreBtn.Foreground = new SolidColorBrush(filter == "barre" ? TextDark : TextLight);
        FilterFavBtn.Background = new SolidColorBrush(filter == "fav" ? FavColor : InactiveBg);
        FilterFavBtn.Foreground = new SolidColorBrush(filter == "fav" ? TextDark : TextLight);
        UpdateChord();
    }

    // ── Кнопки выбора ────────────────────────────────────────

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
            var btn = MakeButton(label, 0, 38);
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
            Content = text, Height = height,
            FontSize = 14, FontWeight = FontWeights.Bold,
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
            btn.Background = new SolidColorBrush(active ? AccentColor : InactiveBg);
            btn.Foreground = new SolidColorBrush(active ? TextDark : TextLight);
        }
    }

    private void HighlightTypeButtons()
    {
        foreach (var btn in _typeButtons)
        {
            bool active = btn.Tag?.ToString() == _selectedType;
            btn.Background = new SolidColorBrush(active ? AccentColor : InactiveBg);
            btn.Foreground = new SolidColorBrush(active ? TextDark : TextLight);
        }
    }

    // ── Обновление аккорда ───────────────────────────────────

    private void UpdateChord()
    {
        // Получаем все варианты
        var all = ChordLibrary.GetVoicings(_selectedRoot, _selectedType);

        // Фильтруем
        _voicings = _filter switch
        {
            "easy" => all.Where(c => c.Difficulty == ChordDifficulty.Easy).ToList(),
            "barre" => all.Where(c => c.Difficulty == ChordDifficulty.Medium || c.BaseFret > 1).ToList(),
            "fav" => ChordLibrary.IsFavorite(_selectedRoot, _selectedType) ? all : Array.Empty<ChordDefinition>(),
            _ => all
        };

        _currentVoicingIndex = 0;
        ShowCurrentVoicing();
    }

    private void ShowCurrentVoicing()
    {
        if (_voicings.Count == 0)
        {
            _currentChord = null;
            ChordNameLabel.Text = $"{_selectedRoot}?";
            TypeNameLabel.Text = "";
            FormulaLabel.Text = "";
            NotesLabel.Text = "";
            PositionLabel.Text = "—";
            FretInfoLabel.Text = _filter == "fav" ? "Аккорд не в избранном"
                : _filter == "easy" ? "Нет простых вариантов" : "Не найден";
            InfoLabel.Text = "";
            DiagramCanvas.Children.Clear();
            UpdateFavButton();
            return;
        }

        _currentVoicingIndex = Math.Clamp(_currentVoicingIndex, 0, _voicings.Count - 1);
        _currentChord = _voicings[_currentVoicingIndex];

        // Имя и теория
        ChordNameLabel.Text = _currentChord.DisplayName;
        TypeNameLabel.Text = ChordTheory.GetTypeName(_selectedType);
        FormulaLabel.Text = ChordTheory.GetFormulaString(_selectedType);
        NotesLabel.Text = string.Join("   ", ChordTheory.GetNotes(_selectedRoot, _selectedType));

        // Позиция
        PositionLabel.Text = $"{_currentVoicingIndex + 1} / {_voicings.Count}";
        FretInfoLabel.Text = _currentChord.PositionLabel;

        // Сложность
        string diff = _currentChord.Difficulty switch
        {
            ChordDifficulty.Easy => "🟢 Простой",
            ChordDifficulty.Medium => "🟡 Средний",
            ChordDifficulty.Hard => "🔴 Сложный",
            _ => ""
        };
        string fretsStr = string.Join(" ", _currentChord.Frets.Select(f => f < 0 ? "x" : f.ToString()));
        InfoLabel.Text = $"Аппликатура: {fretsStr}\nСложность: {diff}";

        UpdateFavButton();
        DrawDiagram(_currentChord);
    }

    // ── Позиции ──────────────────────────────────────────────

    private void PrevPosition_Click(object s, RoutedEventArgs e)
    {
        if (_voicings.Count > 0)
        {
            _currentVoicingIndex = (_currentVoicingIndex - 1 + _voicings.Count) % _voicings.Count;
            ShowCurrentVoicing();
        }
    }

    private void NextPosition_Click(object s, RoutedEventArgs e)
    {
        if (_voicings.Count > 0)
        {
            _currentVoicingIndex = (_currentVoicingIndex + 1) % _voicings.Count;
            ShowCurrentVoicing();
        }
    }

    // ── Избранное ────────────────────────────────────────────

    private void Fav_Click(object s, RoutedEventArgs e)
    {
        ChordLibrary.ToggleFavorite(_selectedRoot, _selectedType);
        UpdateFavButton();
    }

    private void UpdateFavButton()
    {
        bool isFav = ChordLibrary.IsFavorite(_selectedRoot, _selectedType);
        FavButton.Content = isFav ? "★" : "☆";
        FavButton.Foreground = new SolidColorBrush(isFav ? FavColor : Color.FromRgb(124, 111, 150));
    }

    // ── Диаграмма ────────────────────────────────────────────

    private void DrawDiagram(ChordDefinition chord)
    {
        DiagramCanvas.Children.Clear();

        double canvasW = DiagramCanvas.Width;
        double canvasH = DiagramCanvas.Height;

        double leftPad = 30;
        double topPad = 30;
        double gridW = canvasW - leftPad - 20;
        double gridH = canvasH - topPad - 20;
        int fretCount = 5;

        double stringSpacing = gridW / 5.0;
        double fretSpacing = gridH / fretCount;

        // Порожек или номер лада
        if (chord.BaseFret <= 1)
        {
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
            var label = new TextBlock
            {
                Text = chord.BaseFret.ToString(),
                FontSize = 14, Foreground = new SolidColorBrush(TextLight),
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(label, 4);
            Canvas.SetTop(label, topPad + fretSpacing * 0.3);
            DiagramCanvas.Children.Add(label);
        }

        // Лады
        for (int f = 0; f <= fretCount; f++)
        {
            double y = topPad + f * fretSpacing;
            DiagramCanvas.Children.Add(new Line
            {
                X1 = leftPad, Y1 = y, X2 = leftPad + gridW, Y2 = y,
                Stroke = new SolidColorBrush(Color.FromRgb(90, 72, 110)),
                StrokeThickness = f == 0 && chord.BaseFret > 1 ? 2 : 1
            });
        }

        // Струны
        for (int s = 0; s < 6; s++)
        {
            double x = leftPad + s * stringSpacing;
            DiagramCanvas.Children.Add(new Line
            {
                X1 = x, Y1 = topPad, X2 = x, Y2 = topPad + gridH,
                Stroke = new SolidColorBrush(StringColor),
                StrokeThickness = 1.5 - s * 0.1
            });
        }

        // Баррэ
        if (chord.BaseFret > 1)
        {
            int barreFret = chord.Frets.Where(f => f >= 0).DefaultIfEmpty(0).Min();
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
                DiagramCanvas.Children.Add(new Line
                {
                    X1 = leftPad + firstStr * stringSpacing,
                    Y1 = y,
                    X2 = leftPad + lastStr * stringSpacing,
                    Y2 = y,
                    Stroke = new SolidColorBrush(DotColor),
                    StrokeThickness = 8,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    Opacity = 0.8
                });
            }
        }

        // Точки + X/O
        for (int s = 0; s < 6; s++)
        {
            double x = leftPad + s * stringSpacing;
            int fret = chord.Frets[s];

            if (fret < 0)
            {
                var mark = new TextBlock
                {
                    Text = "✕", FontSize = 16,
                    Foreground = new SolidColorBrush(MutedColor),
                    FontWeight = FontWeights.Bold
                };
                Canvas.SetLeft(mark, x - 7);
                Canvas.SetTop(mark, topPad - 25);
                DiagramCanvas.Children.Add(mark);
            }
            else if (fret == 0)
            {
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
                int relativeFret = fret - chord.BaseFret + 1;
                relativeFret = Math.Clamp(relativeFret, 1, fretCount);
                double y = topPad + (relativeFret - 0.5) * fretSpacing;
                double dotSize = 18;

                DiagramCanvas.Children.Add(new Ellipse
                {
                    Width = dotSize, Height = dotSize,
                    Fill = new SolidColorBrush(DotColor)
                });
                Canvas.SetLeft(DiagramCanvas.Children[^1], x - dotSize / 2);
                Canvas.SetTop(DiagramCanvas.Children[^1], y - dotSize / 2);
            }
        }

        // Подписи струн
        string[] stringNames = { "E", "A", "D", "G", "B", "e" };
        for (int s = 0; s < 6; s++)
        {
            double x = leftPad + s * stringSpacing;
            var label = new TextBlock
            {
                Text = stringNames[s], FontSize = 11,
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
        if (_currentChord == null || _audioHost == null) return;
        float[] samples = ChordPlayer.Synthesize(_currentChord, _audioHost.SampleRate);
        _audioHost.PlaySamples(samples);
    }
}
