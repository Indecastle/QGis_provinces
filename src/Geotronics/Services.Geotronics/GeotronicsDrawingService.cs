using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Geotronics.DataAccess;
using Geotronics.Utils;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.Triangulate;
using NetTopologySuite.Triangulate.Polygon;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Point = SixLabors.ImageSharp.Point;

namespace Geotronics.Services.Geotronics;

public interface IGeotronicsDrawingService
{
    Task<Stream> GenerateImage(int resolution, int? offset, int? limit, double dotSize, TriangulationType triangulationType);
}

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class GeotronicsDrawingService : IGeotronicsDrawingService
{
    private readonly Pen _black_pen = new SolidPen(Color.Black);

    private readonly IGeotronicsService _geotronicsService;
    private readonly AppDbContext _appDbContext;

    public GeotronicsDrawingService(IGeotronicsService geotronicsService, AppDbContext appDbContext)
    {
        _geotronicsService = geotronicsService;
        _appDbContext = appDbContext;
    }

    public async Task<Stream> GenerateImage(int resolution, int? offset, int? limit, double dotSize, TriangulationType triangulationType)
    {
        var provinces = await _appDbContext.Regions.ToArrayAsync();
        var allCoordinates = provinces.SelectMany(x => x.Geom.Coordinates).ToArray();
        var data = new DrawingDataSource(
            GeometryUtils.MinPointOnTheCoordinates(allCoordinates),
            GeometryUtils.MaxPointOnTheCoordinates(allCoordinates),
            resolution,
            dotSize,
            offset,
            limit);

        Image image = new Image<Rgba32>(data.ImageSize, (int)(data.ImageSize * data.AspectRatio));
        // Graphics graph = Graphics.FromImage(image);
        // graph.Clear(Color.Azure);
        image.Mutate(x => x.Clear(Color.Azure));

        if (triangulationType == TriangulationType.FilledProvinces)
            await DrawTriangulationProvinces(image, provinces, data, triangulationType);
        else
            DrawProvinces(image, provinces, data);

        if (triangulationType == TriangulationType.RandomPoints || triangulationType == TriangulationType.FilledRandomPoints)
            await DrawTriangulationPoints(image, data, triangulationType);
        else
            await DrawPoints(image, data);

        image.Mutate(x => x.RotateFlip(RotateMode.Rotate180, FlipMode.Horizontal));
        // image.RotateFlip(RotateFlipType.Rotate180FlipX);
        return CreateMemoryStream(image);
    }

    private async Task DrawTriangulationProvinces(Image image, Regions[] provinces, DrawingDataSource data, TriangulationType triangulationType)
    {
        var geometriesGroup = provinces
            .Select(x => new PolygonTriangulator(x.Geom).GetResult() as GeometryCollection)
            .Select(x => x.Geometries);

        var pointsGroups = new ConcurrentBag<PointF[]>();
        Parallel.ForEach(geometriesGroup, geometries =>
        {
            foreach (var geometry in geometries)
                pointsGroups.Add(GetPolygon(geometry.Coordinates, data));
        });
        
        var fillMode = triangulationType == TriangulationType.FilledProvinces
            ? FillPolygonMode.Random
            : FillPolygonMode.NoFill;
        
        DrawPolygons(image, pointsGroups, data, fillMode);
    }

    private void DrawProvinces(Image image, Regions[] provinces, DrawingDataSource data)
    {
        var pointsGroups = provinces.Select(x => GetPolygon(x.Geom!.Coordinates, data));
        DrawPolygons(image, pointsGroups, data, FillPolygonMode.Gradient);
    }

    private async Task DrawPoints(Image image, DrawingDataSource data)
    {
        var points = await _geotronicsService.GetAllAsync(data.Offset, data.Limit);
        var relativePoints =
            points.ToLookup(x => x.RegionId, x => data.GetCenterPoint(x.Coordinate.X, x.Coordinate.Y));

        var nextBrush = DrawingUtils.RandomBrushEnumerator().GetEnumerator();

        foreach (var provincePoints in relativePoints)
        {
            nextBrush.MoveNext();
            var brush = nextBrush.Current;
            foreach (var point in provincePoints)
            {
                EllipsePolygon circle = new EllipsePolygon(point.X - data.HalfDotRadius, point.Y - data.HalfDotRadius, data.DotRadius);
                image.Mutate(x => x.FillPolygon(brush, circle.Points.ToArray()));
                // graph.FillEllipse(brush, point.X - data.HalfDotRadius, point.Y - data.HalfDotRadius, data.DotRadius, data.DotRadius);
            }
        }
    }
    
    private async Task DrawTriangulationPoints(Image image, DrawingDataSource data, TriangulationType triangulationType)
    {
        var points = await _geotronicsService.GetAllAsync(data.Offset, data.Limit);

        var vtb = new DelaunayTriangulationBuilder();
        vtb.SetSites(points.Select(x => x.Coordinate.Coordinate).ToArray());
        
        var geomColl = vtb.GetTriangles(new GeometryFactory());
        var pointsGroups = geomColl.Select(x => GetPolygon(x.Coordinates, data)).ToArray();

        var fillMode = triangulationType == TriangulationType.FilledRandomPoints
            ? FillPolygonMode.Random
            : FillPolygonMode.NoFill;
        
        DrawPolygons(image, pointsGroups, data, fillMode);
    }

    private PointF[] GetPolygon(Coordinate[] coordinates, DrawingDataSource data) =>
        coordinates.Select(c => data.GetCenterPoint(c.X, c.Y)).ToArray();

    private void DrawPolygons(Image image, IEnumerable<PointF[]> pointsGroups, DrawingDataSource data,
        FillPolygonMode fillMode = FillPolygonMode.NoFill)
    {
        if (fillMode != FillPolygonMode.NoFill)
        {
            switch (fillMode)
            {
                case FillPolygonMode.Gradient:
                    foreach (var points in pointsGroups)
                        image.Mutate(x => x.FillPolygon(data.GradientBrush, points));
                        // graph.FillPolygon(data.GradientBrush, points);
                    break;
                case FillPolygonMode.Random:
                    var brushEnumerator = DrawingUtils.RandomBrushEnumerator().GetEnumerator();
                    foreach (var points in pointsGroups)
                    {
                        brushEnumerator.MoveNext();
                        image.Mutate(x => x.FillPolygon(brushEnumerator.Current, points));
                    }

                    break;
            }
        }

        foreach (var points in pointsGroups)
            image.Mutate(x => x.DrawPolygon(_black_pen, points));
    }

    private MemoryStream CreateMemoryStream(Image image)
    {
        var memoryStream = new MemoryStream();
        image.Save(memoryStream, new PngEncoder());
        memoryStream.Position = 0;

        return memoryStream;
    }
}