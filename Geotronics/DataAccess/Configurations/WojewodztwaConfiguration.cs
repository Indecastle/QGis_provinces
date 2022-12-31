using Geotronics.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Geotronics.DataAccess.Configurations;

public class WojewodztwaConfiguration : IEntityTypeConfiguration<Wojewodztwa>
{
    public void Configure(EntityTypeBuilder<Wojewodztwa> builder)
    {
        builder.ToTable("wojewodztwa");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("gid");
        builder.Property(x => x.Geom).HasColumnName("geom");
    }
}