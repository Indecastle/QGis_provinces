using System.Data;
using FluentMigrator;

namespace Geotronics.Migrations;

[Migration(0)]
public class AddTables : Migration {
    public override void Up()
    {
        Create
            .Table("Points")
            .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("Coordinate").AsCustom("geometry(Point)").NotNullable()
            .WithColumn("WojewodztwaId").AsInt32().ForeignKey("wojewodztwa", "gid").NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Points");
    }
}