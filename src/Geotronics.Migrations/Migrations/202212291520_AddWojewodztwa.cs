using System.Data;
using FluentMigrator;

namespace Geotronics.Migrations;

[Migration(202212291520)]
public class AddWojewodztwa : Migration {
    public override void Up()
    {
        Alter.Column("Coordinate").OnTable("Points").AsCustom("geometry(PointZ)");
    }

    public override void Down()
    {
        Delete.FromTable("Points").AllRows();
        Alter.Column("Coordinate").OnTable("Points").AsCustom("geometry(Point)");
    }
}