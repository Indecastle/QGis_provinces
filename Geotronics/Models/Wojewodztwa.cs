

using NetTopologySuite.Geometries;

public class Wojewodztwa
{
    public Wojewodztwa()
    {
    }
    
    public Wojewodztwa(int id, Geometry? geom)
    {
        Id = id;
        Geom = geom;
    }

    public int Id { get; set; }
    public Geometry? Geom { get; set; }
}