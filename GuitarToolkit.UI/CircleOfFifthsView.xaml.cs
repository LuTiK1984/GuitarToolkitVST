using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GuitarToolkit.Core.DSP;
using GuitarToolkit.Core.Models;

namespace GuitarToolkit.UI;

public partial class CircleOfFifthsView : UserControl
{
    private IAudioPlayback? _audio;
    private int _selectedIndex = 0;
    private bool _showMinor = false;

    private static readonly string[] NoteNames =
        { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    private static readonly int[] MajorScale = { 0, 2, 4, 5, 7, 9, 11 };
    private static readonly int[] MinorScale = { 0, 2, 3, 5, 7, 8, 10 };

    private static readonly Color AccentColor = Color.FromRgb(203, 166, 247);
    private static readonly Color InactiveBg = Color.FromRgb(74, 56, 96);
    private static readonly Color TextLight = Color.FromRgb(205, 214, 244);
    private static readonly Color TextDark = Color.FromRgb(26, 21, 37);
    private static readonly Color DimText = Color.FromRgb(124, 111, 150);
    private static readonly Color RingColor = Color.FromRgb(45, 34, 64);
    private static readonly Color GreenColor = Color.FromRgb(166, 227, 161);

    public CircleOfFifthsView()
    {
        InitializeComponent();
        Loaded += (s, e) => { DrawCircle(); UpdateInfo(); };
    }

    public void Initialize(IAudioPlayback audio)
    {
        _audio = audio;
    }

    private static string FlatToSharp(string note)
    {
        return note switch
        {
            "Db" => "C#", "Eb" => "D#", "Gb" => "F#",
            "Ab" => "G#", "Bb" => "A#",
            "Bbm" => "A#", "Dbm" => "C#", "Ebm" => "D#",
            "Gbm" => "F#", "Abm" => "G#",
            _ => note
        };
    }

    /// <summary>
    /// Извлекает тонику из названия минорной тональности ("Am" → "A", "F#m" → "F#").
    /// </summary>
    private static string MinorRoot(string minorKey)
    {
        return minorKey.EndsWith("m") ? minorKey[..^1] : minorKey;
    }

    private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        DrawCircle();
    }

    private void DrawCircle()
    {
        CircleCanvas.Children.Clear();

        double w = CircleCanvas.ActualWidth;
        double h = CircleCanvas.ActualHeight;
        if (w < 100 || h < 100) return;

        double cx = w / 2;
        double cy = h / 2;
        double outerR = Math.Min(cx, cy) - 20;
        double innerR = outerR * 0.62;

        // Внешнее кольцо
        var outerRing = new Ellipse
        {
            Width = outerR * 2, Height = outerR * 2,
            Fill = new SolidColorBrush(RingColor),
            Stroke = new SolidColorBrush(Color.FromRgb(90, 72, 110)),
            StrokeThickness = 2
        };
        Canvas.SetLeft(outerRing, cx - outerR);
        Canvas.SetTop(outerRing, cy - outerR);
        CircleCanvas.Children.Add(outerRing);

        // Внутреннее кольцо
        var innerRing = new Ellipse
        {
            Width = innerR * 2, Height = innerR * 2,
            Fill = new SolidColorBrush(Color.FromRgb(37, 29, 56)),
            Stroke = new SolidColorBrush(InactiveBg),
            StrokeThickness = 1
        };
        Canvas.SetLeft(innerRing, cx - innerR);
        Canvas.SetTop(innerRing, cy - innerR);
        CircleCanvas.Children.Add(innerRing);

        int prev = (_selectedIndex + 11) % 12;
        int next = (_selectedIndex + 1) % 12;

        for (int i = 0; i < 12; i++)
        {
            double angle = (i * 30.0 - 90.0) * Math.PI / 180.0;
            bool isSelected = i == _selectedIndex;
            bool isNeighbor = i == prev || i == next;
            int idx = i;

            // ── Мажор (внешний) ──────────────────────────────
            double majorR = (outerR + innerR) / 2 + 6;
            double mx = cx + majorR * Math.Cos(angle);
            double my = cy + majorR * Math.Sin(angle);
            double dotSize = isSelected ? 46 : (isNeighbor ? 42 : 38);

            bool majorActive = isSelected && !_showMinor;
            Color majorColor = majorActive ? AccentColor
                : isNeighbor ? Color.FromRgb(120, 96, 160)
                : InactiveBg;

            var majorDot = new Ellipse
            {
                Width = dotSize, Height = dotSize,
                Fill = new SolidColorBrush(majorColor),
                Cursor = Cursors.Hand
            };
            Canvas.SetLeft(majorDot, mx - dotSize / 2);
            Canvas.SetTop(majorDot, my - dotSize / 2);
            majorDot.MouseDown += (s, e) =>
            {
                _selectedIndex = idx;
                _showMinor = false;
                DrawCircle();
                UpdateInfo();
            };
            CircleCanvas.Children.Add(majorDot);

            var majorText = new TextBlock
            {
                Text = CircleOfFifths.MajorKeys[i],
                FontSize = isSelected ? 16 : 13,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(
                    majorActive || isNeighbor ? TextDark : TextLight),
                TextAlignment = TextAlignment.Center,
                Width = dotSize
            };
            Canvas.SetLeft(majorText, mx - dotSize / 2);
            Canvas.SetTop(majorText, my - (isSelected ? 10 : 8));
            majorText.IsHitTestVisible = false;
            CircleCanvas.Children.Add(majorText);

            // ── Минор (внутренний) ───────────────────────────
            double minorR = innerR * 0.65;
            double mnx = cx + minorR * Math.Cos(angle);
            double mny = cy + minorR * Math.Sin(angle);
            double minDotSize = isSelected ? 38 : 30;

            bool minorActive = isSelected && _showMinor;
            Color minorColor = minorActive ? GreenColor
                : isSelected ? Color.FromRgb(60, 50, 80)
                : Color.FromRgb(45, 34, 64);

            var minorDot = new Ellipse
            {
                Width = minDotSize, Height = minDotSize,
                Fill = new SolidColorBrush(minorColor),
                Cursor = Cursors.Hand
            };
            Canvas.SetLeft(minorDot, mnx - minDotSize / 2);
            Canvas.SetTop(minorDot, mny - minDotSize / 2);
            minorDot.MouseDown += (s, e) =>
            {
                _selectedIndex = idx;
                _showMinor = true;
                DrawCircle();
                UpdateInfo();
            };
            CircleCanvas.Children.Add(minorDot);

            var minorText = new TextBlock
            {
                Text = CircleOfFifths.MinorKeys[i],
                FontSize = isSelected ? 13 : 11,
                Foreground = new SolidColorBrush(minorActive ? TextDark : DimText),
                TextAlignment = TextAlignment.Center,
                Width = minDotSize
            };
            Canvas.SetLeft(minorText, mnx - minDotSize / 2);
            Canvas.SetTop(minorText, mny - (isSelected ? 8 : 7));
            minorText.IsHitTestVisible = false;
            CircleCanvas.Children.Add(minorText);

            // Разделители
            double lineAngle = ((i - 0.5) * 30.0 - 90.0) * Math.PI / 180.0;
            var line = new Line
            {
                X1 = cx + innerR * Math.Cos(lineAngle),
                Y1 = cy + innerR * Math.Sin(lineAngle),
                X2 = cx + outerR * Math.Cos(lineAngle),
                Y2 = cy + outerR * Math.Sin(lineAngle),
                Stroke = new SolidColorBrush(Color.FromRgb(55, 44, 74)),
                StrokeThickness = 0.5, Opacity = 0.5
            };
            CircleCanvas.Children.Add(line);
        }
    }

    private void UpdateInfo()
    {
        int i = _selectedIndex;
        string major = CircleOfFifths.MajorKeys[i];
        string minor = CircleOfFifths.MinorKeys[i];

        if (_showMinor)
        {
            // Минорная тональность
            string root = MinorRoot(minor);
            string sharpRoot = FlatToSharp(root);
            int rootIdx = Array.IndexOf(NoteNames, sharpRoot);
            if (rootIdx < 0) rootIdx = 0;

            KeyNameLabel.Text = $"{root} минор";
            MinorLabel.Text = $"Параллельный мажор: {major}";
            SignatureLabel.Text = CircleOfFifths.GetKeySignatureText(i);

            string notes = string.Join("  ",
                MinorScale.Select(s => NoteNames[(rootIdx + s) % 12]));
            NotesLabel.Text = notes;

            // Минорные диатонические аккорды
            var chords = ProgressionBuilder.GetDiatonicChords(sharpRoot, modeIndex: 1);
            DisplayChords(chords);

            // Прогрессии для минора
            string prog1 = $"{chords[0].Root}m – {chords[5].Root} – {chords[2].Root} – {chords[6].Root}";
            string prog2 = $"{chords[0].Root}m – {chords[3].Root}m – {chords[4].Root}m – {chords[0].Root}m";
            string prog3 = $"{chords[0].Root}m – {chords[6].Root} – {chords[5].Root} – {chords[4].Root}";
            ProgressionsLabel.Text = $"i–VI–III–VII:  {prog1}\ni–iv–v–i:  {prog2}\ni–VII–VI–v:  {prog3}";

            int prev = (i + 11) % 12;
            int next = (i + 1) % 12;
            RelatedLabel.Text = $"Параллельный мажор:  {major}\n"
                              + $"Субдоминанта:  {CircleOfFifths.MinorKeys[prev]}\n"
                              + $"Доминанта:  {CircleOfFifths.MinorKeys[next]}\n"
                              + $"Энгармонический:  {GetEnharmonic(root)}";
        }
        else
        {
            // Мажорная тональность
            string sharpRoot = FlatToSharp(major);
            int rootIdx = Array.IndexOf(NoteNames, sharpRoot);
            if (rootIdx < 0) rootIdx = 0;

            KeyNameLabel.Text = $"{major} мажор";
            MinorLabel.Text = $"Параллельный минор: {minor}";
            SignatureLabel.Text = CircleOfFifths.GetKeySignatureText(i);

            string notes = string.Join("  ",
                MajorScale.Select(s => NoteNames[(rootIdx + s) % 12]));
            NotesLabel.Text = notes;

            var chords = CircleOfFifths.GetChords(i);
            DisplayChords(chords);

            string c(ProgressionStep s) => s.Root + (s.ChordType == "Major" ? "" : s.ChordType);
            string prog1 = $"{c(chords[0])} – {c(chords[4])} – {c(chords[5])} – {c(chords[3])}";
            string prog2 = $"{c(chords[0])} – {c(chords[3])} – {c(chords[4])} – {c(chords[0])}";
            string prog3 = $"{c(chords[1])} – {c(chords[4])} – {c(chords[0])}";
            ProgressionsLabel.Text = $"I–V–vi–IV:  {prog1}\nI–IV–V–I:  {prog2}\nii–V–I:  {prog3}";

            int prev = (i + 11) % 12;
            int next = (i + 1) % 12;
            RelatedLabel.Text = $"IV (субдоминанта):  {CircleOfFifths.MajorKeys[prev]} мажор\n"
                              + $"V (доминанта):  {CircleOfFifths.MajorKeys[next]} мажор\n"
                              + $"Параллельный минор:  {minor}\n"
                              + $"Энгармонический:  {GetEnharmonic(major)}";
        }
    }

    private void DisplayChords(ProgressionStep[] chords)
    {
        ChordsDisplay.Items.Clear();

        foreach (var step in chords)
        {
            string suffix = step.ChordType == "Major" ? "" : step.ChordType;
            var border = new Border
            {
                Background = new SolidColorBrush(InactiveBg),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(3)
            };

            var stack = new StackPanel();
            stack.Children.Add(new TextBlock
            {
                Text = step.Degree, FontSize = 10,
                Foreground = new SolidColorBrush(AccentColor),
                HorizontalAlignment = HorizontalAlignment.Center
            });
            stack.Children.Add(new TextBlock
            {
                Text = step.Root + suffix, FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(TextLight),
                HorizontalAlignment = HorizontalAlignment.Center
            });
            border.Child = stack;
            ChordsDisplay.Items.Add(border);
        }
    }

    private string GetEnharmonic(string note)
    {
        return note switch
        {
            "C#" => "Db", "Db" => "C#",
            "D#" => "Eb", "Eb" => "D#",
            "F#" => "Gb", "Gb" => "F#",
            "G#" => "Ab", "Ab" => "G#",
            "A#" => "Bb", "Bb" => "A#",
            _ => "—"
        };
    }

    private void PlayScale_Click(object sender, RoutedEventArgs e)
    {
        if (_audio == null) return;

        string key;
        int[] scale;

        if (_showMinor)
        {
            key = MinorRoot(CircleOfFifths.MinorKeys[_selectedIndex]);
            scale = MinorScale;
        }
        else
        {
            key = CircleOfFifths.MajorKeys[_selectedIndex];
            scale = MajorScale;
        }

        string sharpKey = FlatToSharp(key);
        int rootIdx = Array.IndexOf(NoteNames, sharpKey);
        if (rootIdx < 0) rootIdx = 0;

        int baseMidi = 60 + rootIdx;
        if (baseMidi > 72) baseMidi -= 12;

        int sr = _audio.SampleRate;
        var allSamples = new List<float>();

        // Гамма вверх + октава
        foreach (int interval in scale.Append(12))
        {
            float freq = 440f * MathF.Pow(2f, (baseMidi + interval - 69) / 12f);
            float[] note = NoteSynth.GenerateNote(freq, sr, duration: 0.4f, volume: 0.25f);
            allSamples.AddRange(note);
            allSamples.AddRange(new float[(int)(sr * 0.05f)]);
        }

        _audio.PlaySamples(allSamples.ToArray());
    }
}
