using Geotronics.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Geotronics.DataAccess.Configurations;

public class WojewodztwaConfiguration : IEntityTypeConfiguration<Regions>
{
    public void Configure(EntityTypeBuilder<Regions> builder)
    {
        builder.ToTable("regions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("gid");
        builder.Property(x => x.Geom).HasColumnName("geom");
    }
}