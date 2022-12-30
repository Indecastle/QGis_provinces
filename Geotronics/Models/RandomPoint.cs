using NetTopologySuite.Geometries;

namespace Geotronics.Models;

public class RandomPoint: BaseEntity
{
    public RandomPoint()
    {
    }
    
    public RandomPoint(Guid id, int wojewodztwaId, Point randomPoint)
    {
        Id = id;
        WojewodztwaId = wojewodztwaId;
        Coordinate = randomPoint;
    }
    
    public int WojewodztwaId { get; set; }
    public Point Coordinate { get; set; }
    
    public static RandomPoint New(int wojewodztwaId, Point point)
    {
        return new RandomPoint(Guid.NewGuid(), wojewodztwaId, point);
    }
}