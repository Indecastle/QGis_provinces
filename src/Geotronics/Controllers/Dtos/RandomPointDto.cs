using Geotronics.Models;

namespace Geotronics.Controllers.Dtos;

public class RandomPointDto
{
    public RandomPointDto(RandomPoint point)
    {
        Id = point.Id;
        WojewodztwaId = point.RegionId;
        X = point.Coordinate.X;
        Y = point.Coordinate.Y;
        Z = point.Coordinate.Z;
    }

    public Guid Id { get; }
    public int WojewodztwaId { get; }
    public double X { get; }
    public double Y { get; }
    public double Z { get; }
}