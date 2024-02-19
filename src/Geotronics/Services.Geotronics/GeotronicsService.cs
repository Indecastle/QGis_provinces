using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Geotronics.DataAccess;
using Geotronics.Models;
using Geotronics.Utils;
using static System.Linq.Enumerable;

namespace Geotronics.Services.Geotronics;

public interface IGeotronicsService
{
    Task<RandomPoint[]> GetAllAsync(int? offset = 0, int? limit = 10);
    Task GeneratePointsAsync(int count);
    Task ClearTableAsync();
}

public class GeotronicsService : IGeotronicsService
{
    private readonly AppDbContext _dbContext;

    public GeotronicsService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<RandomPoint[]> GetAllAsync(int? offset = null, int? limit = 10)
    {
        var query = _dbContext.Points.AsQueryable();

        if (offset.HasValue)
            query = query.Skip(offset.Value);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        return query.ToArrayAsync();
    }

    public async Task GeneratePointsAsync(int count)
    {
        var provinces = await _dbContext.Regions.Select(x => x).ToArrayAsync();
        var rand = new Random();

        var points = Range(0, count)
            .Select(_ => GeometryUtils.GeneratePointInsidePolygon(provinces[rand.Next(0, provinces.Length)], rand)).ToArray();
        
        await _dbContext.BulkInsertAsync(points);
        await _dbContext.BulkSaveChangesAsync();
    }

    public async Task ClearTableAsync()
    {
        _dbContext.Points.RemoveRange(_dbContext.Points);
        await _dbContext.SaveChangesAsync();
    }
}