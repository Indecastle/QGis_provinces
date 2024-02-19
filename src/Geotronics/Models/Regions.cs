

using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Prepared;

public class Regions
{
    public Regions()
    {
        _lazyPreparedGeometry = new Lazy<IPreparedGeometry>(() => PreparedGeometryFactory.Prepare(Geom));
    }
    
    public Regions(int id, Geometry? geom) : base()
    {
        Id = id;
        Geom = geom;
    }

    public int Id { get; set; }
    public Geometry? Geom { get; set; }
    public IPreparedGeometry PreparedGeometry => _lazyPreparedGeometry.Value;
    private readonly Lazy<IPreparedGeometry> _lazyPreparedGeometry;
}