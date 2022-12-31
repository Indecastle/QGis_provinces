using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ColorMine.ColorSpaces;
using Geotronics.DataAccess;
using Geotronics.Utils;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.Triangulate.Polygon;
using Point = System.Drawing.Point;

namespace Geotronics.Services.Geotronics;

public interface IGeotronicsDrawingService
{
    Task<Stream> GenerateImage(int resolution, int? offset, int? limit, double dotSize, bool triangulate);
}

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class GeotronicsDrawingService : IGeotronicsDrawingService
{
    private readonly Pen _black_pen = new(Brushes.Black);

    private readonly IGeotronicsService _geotronicsService;
    private readonly AppDbContext _appDbContext;

    public GeotronicsDrawingService(IGeotronicsService geotronicsService, AppDbContext appDbContext)
    {
        _geotronicsService = geotronicsService;
        _appDbContext = appDbContext;
    }

    public async Task<Stream> GenerateImage(int resolution, int? offset, int? limit, double dotSize, bool triangulate)
    {
        var provinces = await _appDbContext.Prowincje.ToArrayAsync();
        var allCoordinates = provinces.SelectMany(x => x.Geom.Coordinates).ToArray();
        var data = new DrawingDataSource(
            GeometryUtils.MinPointOnTheCoordinates(allCoordinates),
            GeometryUtils.MaxPointOnTheCoordinates(allCoordinates),
            resolution,
            dotSize,
            offset,
            limit);

        Image image = new Bitmap(data.ImageSize, (int)(data.ImageSize * data.AspectRatio));
        Graphics graph = Graphics.FromImage(image);
        graph.Clear(Color.Azure);

        if (triangulate)
            await DrawTriangulation(graph, provinces, data);
        else
            DrawProvinces(graph, provinces, data);

        await DrawPoints(graph, data);

        image.RotateFlip(RotateFlipType.Rotate180FlipX);
        return CreateMemoryStream(image);
    }

    private async Task DrawTriangulation(Graphics graph, Wojewodztwa[] provinces, DrawingDataSource data)
    {
        var geometriesGroup = provinces
            .Select(x => new PolygonTriangulator(x.Geom).GetResult() as GeometryCollection)
            .Select(x => x.Geometries);

        var pointsGroups = new ConcurrentBag<Point[]>();
        Parallel.ForEach(geometriesGroup, geometries =>
        {
            foreach (var geometry in geometries)
                pointsGroups.Add(GetPolygon(geometry.Coordinates, data));
        });

        DrawPolygons(graph, pointsGroups, data, FillPolygonType.Random);
    }

    private void DrawProvinces(Graphics graph, Wojewodztwa[] provinces, DrawingDataSource data)
    {
        var pointsGroups = provinces.Select(x => GetPolygon(x.Geom!.Coordinates, data));
        DrawPolygons(graph, pointsGroups, data, FillPolygonType.Gradient);
    }

    private async Task DrawPoints(Graphics graph, DrawingDataSource data)
    {
        var points = await _geotronicsService.GetAllAsync(data.Offset, data.Limit);
        var relativePoints =
            points.ToLookup(x => x.WojewodztwaId, x => data.GetCenterPoint(x.Coordinate.X, x.Coordinate.Y));

        var nextBrush = DrawingUtils.RandomBrushEnumerator().GetEnumerator();

        foreach (var provincePoints in relativePoints)
        {
            nextBrush.MoveNext();
            var brush = nextBrush.Current;
            foreach (var point in provincePoints)
                graph.FillEllipse(brush, point.X - data.HalfDotRadius, point.Y - data.HalfDotRadius, data.DotRadius,
                    data.DotRadius);
        }
    }

    private Point[] GetPolygon(Coordinate[] coordinates, DrawingDataSource data) =>
        coordinates.Select(c => data.GetCenterPoint(c.X, c.Y)).ToArray();

    private void DrawPolygons(Graphics graph, IEnumerable<Point[]> pointsGroups, DrawingDataSource data,
        FillPolygonType fillType = FillPolygonType.NoFill)
    {
        if (fillType != FillPolygonType.NoFill)
        {
            switch (fillType)
            {
                case FillPolygonType.Gradient:
                    foreach (var points in pointsGroups)
                        graph.FillPolygon(data.GradientBrush, points);
                    break;
                case FillPolygonType.Random:
                    var brushEnumerator = DrawingUtils.RandomBrushEnumerator().GetEnumerator();
                    foreach (var points in pointsGroups)
                    {
                        brushEnumerator.MoveNext();
                        graph.FillPolygon(brushEnumerator.Current, points);
                    }

                    break;
            }
        }

        foreach (var points in pointsGroups)
            graph.DrawPolygon(_black_pen, points);
    }

    private MemoryStream CreateMemoryStream(Image image)
    {
        var memoryStream = new MemoryStream();
        image.Save(memoryStream, ImageFormat.Png);
        memoryStream.Position = 0;

        return memoryStream;
    }
}