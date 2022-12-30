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
        builder.Property(x => x.GmlId).HasColumnName("gml_id");
        builder.Property(x => x.JptSjrKo).HasColumnName("jpt_sjr_ko");
        builder.Property(x => x.JptPowier).HasColumnName("jpt_powier");
        builder.Property(x => x.JptKodJe).HasColumnName("jpt_kod_je");
        builder.Property(x => x.Geom).HasColumnName("geom");
    }
}