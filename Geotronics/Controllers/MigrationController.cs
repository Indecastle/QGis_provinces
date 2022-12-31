using FluentMigrator.Runner;
using Microsoft.AspNetCore.Mvc;

namespace Geotronics.Controllers;

[ApiController]
[Route("[controller]")]
public class MigrationController : ControllerBase
{
    private readonly IMigrationRunner _migrationRunner;

    public MigrationController(IMigrationRunner migrationRunner)
    {
        _migrationRunner = migrationRunner;
    }

    [HttpPost("MigrateUp")]
    public void MigrateUp()
    {
        _migrationRunner.MigrateUp();
    }
    
    [HttpPost("Rollback")]
    public void Rollback()
    {
        _migrationRunner.Rollback(1);
    }
}