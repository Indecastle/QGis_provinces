using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using NetTopologySuite.Geometries;
using Point = System.Drawing.Point;

namespace Geotronics.Services.Geotronics;

public class DrawingDataSource
{
    public const int MAX_IMAGE_SIZE = 20000;
    
    public DrawingDataSource(Coordinate minVec, Coordinate maxVec, int resolution, double dotSize, int? offset, int? limit)
    {
        MinVec = minVec;
        MaxVec = maxVec;
        Width = maxVec.X - minVec.X;
        Height = maxVec.Y - minVec.Y;
        AspectRatio = Width / Height;
        ImageSize = resolution <= MAX_IMAGE_SIZE
            ? resolution
            : throw new InvalidConstraintException("\"Resolution\" parameter must be no more than 20000");
        DotRadius = (int)(ImageSize * dotSize / 200);
        HalfDotRadius = DotRadius / 2;
        Offset = offset;
        Limit = limit;
        GradientBrush = new LinearGradientBrush(
            new Point(0, 0),
            GetCenterPoint(MaxVec.X, MaxVec.Y),
            Color.Chocolate,
            Color.Gold);
    }

    public Coordinate MinVec { get; }
    public Coordinate MaxVec { get; }
    public double Width { get; }
    public double Height { get; }
    public double AspectRatio { get; }
    public int ImageSize { get; }
    public int DotRadius { get; }
    public int HalfDotRadius { get; }
    public int? Offset { get; }
    public int? Limit { get; }
    public LinearGradientBrush GradientBrush { get; }
    
    public Point GetCenterPoint(double x, double y)  {
        return new(
            (int)((x - MinVec.X) / Width * ImageSize),
            (int)((y - MinVec.Y) / Height * ImageSize * AspectRatio));
    }
}