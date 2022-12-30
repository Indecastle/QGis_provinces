using Microsoft.EntityFrameworkCore;
using Geotronics.DataAccess;
using Geotronics.Models;
using NetTopologySuite.Geometries;
using static System.Linq.Enumerable;

namespace Geotronics.Services.Geotronics;

public interface IGeotronicsService
{
    Task<RandomPoint[]> GetAll(int? offset = 0, int? limit = 10);
    Task GeneratePoints();
    Task ClearTable();
}

internal class GeotronicsService : IGeotronicsService
{
    private readonly AppDbContext _dbContext;

    public GeotronicsService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<RandomPoint[]> GetAll(int? offset = null, int? limit = 10)
    {
        var query = _dbContext.Points.AsQueryable();

        if (offset.HasValue)
            query = query.Skip(offset.Value);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        return query.ToArrayAsync();
    }

    public async Task GeneratePoints()
    {
        var provinces = await _dbContext.Prowincje.Select(x => x).ToArrayAsync();
        var rand = new Random();

        var points = Range(0, rand.Next(1000, 1500))
            .Select(n => GeneratePointInsidePolygon(provinces[rand.Next(0, provinces.Length)], rand))
            .ToArray();

        await _dbContext.AddRangeAsync(points);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ClearTable()
    {
        _dbContext.Points.RemoveRange(_dbContext.Points);
        await _dbContext.SaveChangesAsync();
    }

    private RandomPoint GeneratePointInsidePolygon(Wojewodztwa province, Random rand)
    {
        var coordinates = province.Geom!.Coordinates;

        var minVec = MinPointOnThecoordinates(coordinates);
        var maxVec = MaxPointOnThecoordinates(coordinates);
        var point = new Point(0, 0);

        do
        {
            //x = rand.NextDouble() * (MaxVec.X - MinVec.X) + MinVec.X;
            //y = rand.NextDouble() * (MaxVec.Y - MinVec.Y) + MinVec.Y;
            point.X = rand.Next((int)minVec.X, (int)maxVec.X);
            point.Y = rand.Next((int)minVec.Y, (int)maxVec.Y);
        } while (!IsPointInPolygon(coordinates, point));

        return RandomPoint.New(province.Id, point);
    }

    private Coordinate MinPointOnThecoordinates(Coordinate[] coordinates)
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

    private Coordinate MaxPointOnThecoordinates(Coordinate[] coordinates)
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

    private bool IsPointInPolygon(Coordinate[] coordinates, Point point)
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