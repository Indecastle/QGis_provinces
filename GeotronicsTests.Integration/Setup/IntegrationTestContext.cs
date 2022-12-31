using Geotronics.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace GeotronicsTests.Integration.Setup;

public class IntegrationTestContext : IDisposable
{
    public AppDbContext _dbContext;

    [OneTimeSetUp]
    public void IntegrationTestContext_OneTimeSetUp()
    {
        _dbContext = new AppDbContext(new DbContextOptions<AppDbContext>());
    }
    
    [OneTimeTearDown]
    public void IntegrationTestsContext_OneTimeTearDown()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        _dbContext.Dispose();
    }
}