using ColorMine.ColorSpaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;

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
            yield return new SolidBrush(Color.FromRgba((byte)(rgb.R), (byte)(rgb.G), (byte)(rgb.B), alpha));
        }
    }
}