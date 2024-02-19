namespace Geotronics.Models;
using Point = NetTopologySuite.Geometries.Point;

public class RandomPoint: BaseEntity
{
    public RandomPoint()
    {
    }
    
    public RandomPoint(Guid id, int regionId, Point randomPoint)
    {
        Id = id;
        RegionId = regionId;
        Coordinate = randomPoint;
    }
    
    public int RegionId { get; set; }
    public Point Coordinate { get; set; }
    
    public static RandomPoint New(int wojewodztwaId, Point point)
    {
        return new RandomPoint(Guid.NewGuid(), wojewodztwaId, point);
    }
}