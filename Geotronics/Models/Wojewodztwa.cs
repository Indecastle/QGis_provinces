

using NetTopologySuite.Geometries;

public class Wojewodztwa
{
    public Wojewodztwa()
    {
    }
    
    public Wojewodztwa(int id, string? gmlId, string? jptSjrKo, string? jptPowier, string? jptKodJe, Geometry? geom)
    {
        Id = id;
        GmlId = gmlId;
        JptSjrKo = jptSjrKo;
        JptPowier = jptPowier;
        JptKodJe = jptKodJe;
        Geom = geom;
    }

    public int Id { get; set; }
    public string? GmlId { get; set; }
    public string? JptSjrKo { get; set; }
    public string? JptPowier { get; set; }
    public string? JptKodJe { get; set; }
    public Geometry? Geom { get; set; }
}