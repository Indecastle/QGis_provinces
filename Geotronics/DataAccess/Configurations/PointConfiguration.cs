using Geotronics.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Geotronics.DataAccess.Configurations;

public class PointConfiguration : IEntityTypeConfiguration<RandomPoint>
{
    public void Configure(EntityTypeBuilder<RandomPoint> builder)
    {
        builder.ToTable("Points");
        builder.HasKey(x => x.Id);
        //builder.Property(x => x.Coordinate).HasColumnName("coordinate");
        
        builder.HasOne<Wojewodztwa>().WithMany().HasForeignKey(x => x.WojewodztwaId);
    }
}