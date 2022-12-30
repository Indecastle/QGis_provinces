using Geotronics.Controllers.Dtos;
using Microsoft.AspNetCore.Mvc;
using Geotronics.Models;
using Geotronics.Services.Geotronics;

namespace Geotronics.Controllers;

[ApiController]
[Route("[controller]")]
public class GeotronicsController : ControllerBase
{
    private readonly IGeotronicsService _geotronicsService;

    public GeotronicsController(IGeotronicsService geotronicsService)
    {
        _geotronicsService = geotronicsService;
    }
    
    [HttpGet]
    public async Task<RandomPointDto[]> Get(int? offset = 0, int? limit = 10)
    {
        var items = await _geotronicsService.GetAll(offset, limit);
        return items.Select(x => new RandomPointDto(x)).ToArray();
    }

    [HttpPut("generate-points")]
    public async Task GeneratePoints()
    {
        await _geotronicsService.GeneratePoints();
    }
    
    [HttpDelete()]
    public async Task ClearTable()
    {
        await _geotronicsService.ClearTable();
    }
}
