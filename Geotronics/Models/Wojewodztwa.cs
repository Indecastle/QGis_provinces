

using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Prepared;

public class Wojewodztwa
{
    public Wojewodztwa()
    {
        _lazyPreparedGeometry = new Lazy<IPreparedGeometry>(() => PreparedGeometryFactory.Prepare(Geom));
    }
    
    public Wojewodztwa(int id, Geometry? geom) : base()
    {
        Id = id;
        Geom = geom;
    }

    public int Id { get; set; }
    public Geometry? Geom { get; set; }
    public IPreparedGeometry PreparedGeometry => _lazyPreparedGeometry.Value;
    private readonly Lazy<IPreparedGeometry> _lazyPreparedGeometry;
}