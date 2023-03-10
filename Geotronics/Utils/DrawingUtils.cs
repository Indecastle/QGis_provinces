using System.Drawing;
using ColorMine.ColorSpaces;

namespace Geotronics.Utils;

public static class DrawingUtils
{
    private static readonly byte alpha = 150;
    public static IEnumerable<SolidBrush> RandomBrushEnumerator()
    {
        Random rand = new();
        while (true)
        {
            var rgb = new Hsv { H = rand.Next(360), S = 1, V = 1 }.ToRgb();
            yield return new SolidBrush(Color.FromArgb(alpha, (int)(rgb.R), (int)(rgb.G), (int)(rgb.B)));
        }
    }
}