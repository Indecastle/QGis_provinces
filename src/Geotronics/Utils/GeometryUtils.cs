using Geotronics.Models;
using NetTopologySuite.Geometries;
using Point = NetTopologySuite.Geometries.Point;


namespace Geotronics.Utils;

public static class GeometryUtils
{
    public const int MAX_HEIGHT = 300;

    public static RandomPoint GeneratePointInsidePolygon(Regions province, Random rand)
    {
        var coordinates = province.Geom!.Coordinates;

        var minVec = MinPointOnTheCoordinates(coordinates);
        var maxVec = MaxPointOnTheCoordinates(coordinates);
        var point = new Point(0, 0, 0);

        do
        {
            point.X = minVec.X + (maxVec.X - minVec.X) * rand.NextSingle();
            point.Y = minVec.Y + (maxVec.Y - minVec.Y) * rand.NextSingle();
        } while (!province.PreparedGeometry.Contains(point));
        
        point.Z = rand.Next(0, MAX_HEIGHT);
        return RandomPoint.New(province.Id, point);
    }

    public static Coordinate MinPointOnTheCoordinates(Coordinate[] coordinates)
    {
        double minX = coordinates[0].X;
        double minY = coordinates[0].Y;
        for (int i = 1; i < coordinates.Length; i++)
        {
            if (minX > coordinates[i].X)
            {
                minX = coordinates[i].X;
            }

            if (minY > coordinates[i].Y)
            {
                minY = coordinates[i].Y;
            }
        }

        return new Coordinate(minX, minY);
    }

    public static Coordinate MaxPointOnTheCoordinates(Coordinate[] coordinates)
    {
        double maxX = coordinates[0].X;
        double maxY = coordinates[0].Y;
        for (int i = 1; i < coordinates.Length; i++)
        {
            if (maxX < coordinates[i].X)
            {
                maxX = coordinates[i].X;
            }

            if (maxY < coordinates[i].Y)
            {
                maxY = coordinates[i].Y;
            }
        }

        return new Coordinate(maxX, maxY);
    }

    /// <summary>
    /// wiki: https://wrfranklin.org/Research/Short_Notes/pnpoly.html
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static bool IsPointInPolygon(Coordinate[] coordinates, Point point)
    {
        bool isInside = false;
        for (int i = 0, j = coordinates.Length - 1; i < coordinates.Length; j = i++)
        {
            if (((coordinates[i].X > point.X) != (coordinates[j].X > point.X)) &&
                (point.Y < (coordinates[j].Y - coordinates[i].Y) * (point.X - coordinates[i].X) /
                    (coordinates[j].X - coordinates[i].X) + coordinates[i].Y))
            {
                isInside = !isInside;
            }
        }

        return isInside;
    }
}