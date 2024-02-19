using System.Data;
using NetTopologySuite.Geometries;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;


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
        AspectRatio = Height / Width;
        ImageSize = resolution <= MAX_IMAGE_SIZE
            ? resolution
            : throw new InvalidConstraintException("\"Resolution\" parameter must be no more than 20000");
        DotRadius = (int)(ImageSize * dotSize / 400);
        HalfDotRadius = DotRadius / 2;
        Offset = offset;
        Limit = limit;
        GradientBrush = new LinearGradientBrush(
            new PointF(0, 0),
            GetCenterPoint(MaxVec.X, MaxVec.Y),
            GradientRepetitionMode.None,
            new ColorStop(0, Color.Chocolate),
            new ColorStop(1, Color.Gold));
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
    
    public PointF GetCenterPoint(double x, double y)  {
        return new(
            (int)((x - MinVec.X) / Width * ImageSize),
            (int)((y - MinVec.Y) / Height * ImageSize * AspectRatio));
    }
}