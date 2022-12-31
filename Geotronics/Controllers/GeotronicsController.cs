using Geotronics.Controllers.Dtos;
using Microsoft.AspNetCore.Mvc;
using Geotronics.Services.Geotronics;

namespace Geotronics.Controllers;

[ApiController]
[Route("[controller]")]
public class GeotronicsController : ControllerBase
{
    private readonly IGeotronicsService _geotronicsService;
    private readonly IGeotronicsDrawingService _geotronicsDrawingService;

    public GeotronicsController(IGeotronicsService geotronicsService, IGeotronicsDrawingService geotronicsDrawingService)
    {
        _geotronicsService = geotronicsService;
        _geotronicsDrawingService = geotronicsDrawingService;
    }

    [HttpGet]
    public async Task<RandomPointDto[]> GetMany(int? skipPoints = 0, int? takePoints = 10)
    {
        var items = await _geotronicsService.GetAllAsync(skipPoints, takePoints);
        return items.Select(x => new RandomPointDto(x)).ToArray();
    }
    
    /// <response code="400">If the "Resolution" parameter is greater than 20000</response>
    [HttpGet("image")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<FileStreamResult> GetImage(int resolution = 2048, int? skipPoints = 0, int? takePoints = 1000, double dotSize = 1, bool triangulate = false)
    {
        var stream = await _geotronicsDrawingService.GenerateImage(resolution, skipPoints, takePoints, dotSize, triangulate);
        
        return new FileStreamResult(stream, "image/png");
    }

    [HttpPut("generate-points")]
    public async Task GeneratePoints(int count = 1000)
    {
        await _geotronicsService.GeneratePointsAsync(count);
    }

    [HttpDelete]
    public async Task ClearTable()
    {
        await _geotronicsService.ClearTableAsync();
    }
}