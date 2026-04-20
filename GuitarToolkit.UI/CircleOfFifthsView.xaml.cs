using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GuitarToolkit.Core.Models;

namespace GuitarToolkit.UI;

public partial class CircleOfFifthsView : UserControl
{
    private int _selectedIndex = 0;

    private static readonly Color AccentColor = Color.FromRgb(203, 166, 247);
    private static readonly Color InactiveBg = Color.FromRgb(74, 56, 96);
    private static readonly Color TextLight = Color.FromRgb(205, 214, 244);
    private static readonly Color TextDark = Color.FromRgb(26, 21, 37);
    private static readonly Color DimText = Color.FromRgb(124, 111, 150);
    private static readonly Color RingColor = Color.FromRgb(45, 34, 64);

    public CircleOfFifthsView()
    {
        InitializeComponent();
        Loaded += (s, e) => { DrawCircle(); UpdateInfo(); };
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

        // Внешнее кольцо (фон)
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

        // Внутреннее кольцо (миноры)
        var innerRing = new Ellipse
        {
            Width = innerR * 2, Height = innerR * 2,
            Fill = new SolidColorBrush(Color.FromRgb(37, 29, 56)),
            Stroke = new SolidColorBrush(Color.FromRgb(74, 56, 96)),
            StrokeThickness = 1
        };
        Canvas.SetLeft(innerRing, cx - innerR);
        Canvas.SetTop(innerRing, cy - innerR);
        CircleCanvas.Children.Add(innerRing);

        // 12 секторов
        for (int i = 0; i < 12; i++)
        {
            // Угол: 0 = верх (C), по часовой стрелке
            double angle = (i * 30.0 - 90.0) * Math.PI / 180.0;

            bool isSelected = i == _selectedIndex;

            // ── Мажорная тональность (внешнее кольцо) ────────
            double majorR = (outerR + innerR) / 2 + 6;
            double mx = cx + majorR * Math.Cos(angle);
            double my = cy + majorR * Math.Sin(angle);
            double dotSize = isSelected ? 46 : 38;

            var majorDot = new Ellipse
            {
                Width = dotSize, Height = dotSize,
                Fill = new SolidColorBrush(isSelected ? AccentColor : InactiveBg),
                Cursor = Cursors.Hand
            };
            Canvas.SetLeft(majorDot, mx - dotSize / 2);
            Canvas.SetTop(majorDot, my - dotSize / 2);
            int idx = i;
            majorDot.MouseDown += (s, e) => { _selectedIndex = idx; DrawCircle(); UpdateInfo(); };
            CircleCanvas.Children.Add(majorDot);

            var majorText = new TextBlock
            {
                Text = CircleOfFifths.MajorKeys[i],
                FontSize = isSelected ? 16 : 13,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(isSelected ? TextDark : TextLight),
                TextAlignment = TextAlignment.Center,
                Width = dotSize
            };
            Canvas.SetLeft(majorText, mx - dotSize / 2);
            Canvas.SetTop(majorText, my - (isSelected ? 10 : 8));
            majorText.IsHitTestVisible = false;
            CircleCanvas.Children.Add(majorText);

            // ── Минорная тональность (внутреннее кольцо) ─────
            double minorR = innerR * 0.65;
            double mnx = cx + minorR * Math.Cos(angle);
            double mny = cy + minorR * Math.Sin(angle);
            double minDotSize = isSelected ? 38 : 30;

            var minorDot = new Ellipse
            {
                Width = minDotSize, Height = minDotSize,
                Fill = new SolidColorBrush(isSelected
                    ? Color.FromRgb(166, 227, 161)  // зелёный для выбранного
                    : Color.FromRgb(45, 34, 64)),
                Cursor = Cursors.Hand
            };
            Canvas.SetLeft(minorDot, mnx - minDotSize / 2);
            Canvas.SetTop(minorDot, mny - minDotSize / 2);
            minorDot.MouseDown += (s, e) => { _selectedIndex = idx; DrawCircle(); UpdateInfo(); };
            CircleCanvas.Children.Add(minorDot);

            var minorText = new TextBlock
            {
                Text = CircleOfFifths.MinorKeys[i],
                FontSize = isSelected ? 13 : 11,
                Foreground = new SolidColorBrush(isSelected ? TextDark : DimText),
                TextAlignment = TextAlignment.Center,
                Width = minDotSize
            };
            Canvas.SetLeft(minorText, mnx - minDotSize / 2);
            Canvas.SetTop(minorText, mny - (isSelected ? 8 : 7));
            minorText.IsHitTestVisible = false;
            CircleCanvas.Children.Add(minorText);

            // ── Линии-разделители ────────────────────────────
            double lineAngle = ((i - 0.5) * 30.0 - 90.0) * Math.PI / 180.0;
            var line = new Line
            {
                X1 = cx + innerR * Math.Cos(lineAngle),
                Y1 = cy + innerR * Math.Sin(lineAngle),
                X2 = cx + outerR * Math.Cos(lineAngle),
                Y2 = cy + outerR * Math.Sin(lineAngle),
                Stroke = new SolidColorBrush(Color.FromRgb(55, 44, 74)),
                StrokeThickness = 0.5,
                Opacity = 0.5
            };
            CircleCanvas.Children.Add(line);
        }
    }

    private void UpdateInfo()
    {
        int i = _selectedIndex;
        string major = CircleOfFifths.MajorKeys[i];
        string minor = CircleOfFifths.MinorKeys[i];

        KeyNameLabel.Text = $"{major} мажор";
        MinorLabel.Text = $"Параллельный: {minor}";
        SignatureLabel.Text = CircleOfFifths.GetKeySignatureText(i);

        // Диатонические аккорды
        var chords = CircleOfFifths.GetChords(i);
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
                Text = step.Degree,
                FontSize = 10,
                Foreground = new SolidColorBrush(AccentColor),
                HorizontalAlignment = HorizontalAlignment.Center
            });
            stack.Children.Add(new TextBlock
            {
                Text = step.Root + suffix,
                FontSize = 14, FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(TextLight),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            border.Child = stack;
            ChordsDisplay.Items.Add(border);
        }

        // Родственные тональности (соседи по кругу)
        int prev = (i + 11) % 12;
        int next = (i + 1) % 12;
        RelatedLabel.Text = $"Субдоминанта: {CircleOfFifths.MajorKeys[prev]} мажор\n"
                          + $"Доминанта: {CircleOfFifths.MajorKeys[next]} мажор\n"
                          + $"Параллельный минор: {minor}";
    }
}
