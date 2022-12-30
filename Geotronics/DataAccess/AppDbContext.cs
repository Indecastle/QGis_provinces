using System.Reflection;
using Geotronics.Models;
using Microsoft.EntityFrameworkCore;

namespace Geotronics.DataAccess;

public class AppDbContext : DbContext
{
    public virtual DbSet<RandomPoint> Points { get; set; }
    public virtual DbSet<Wojewodztwa> Prowincje { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseNpgsql("User ID=myadmin;Password=123456;Server=localhost;Port=5432;Database=geotronics; Integrated Security=true; Pooling=true;",
            o => o.UseNetTopologySuite());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.HasPostgresExtension("postgis");
        
        base.OnModelCreating(modelBuilder);
    }
}