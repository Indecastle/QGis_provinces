using Geotronics.DataAccess;
using Geotronics.Models;
using Geotronics.Services.Geotronics;
using Geotronics.Utils;
using GeotronicsTests.Integration.Setup;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GeotronicsTests.Integration;

public class Tests : IntegrationTestContext
{
    private const int MIN_DISTANCE = 3000; // 3 kilometers

    private Mock<AppDbContext> _dbContextMock;

    private object[] _entities { get; set; }

    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1000)]
    public async Task Test1(int count)
    {
        IGeotronicsService service = new GeotronicsService(_dbContextMock.Object);
        await service.GeneratePointsAsync(count);
        var points = (_entities as RandomPoint[]).Select(x => x.Coordinate).ToArray();

        for (int i = 0; i < points.Length; i++)
        {
            for (int j = i + 1; j < points.Length - 1; j++)
            {
                var distance = points[i].Distance(points[j]);
                Assert.True(distance > MIN_DISTANCE);
            }
        }
    }

    [SetUp]
    public void Setup()
    {
        _dbContextMock = new Mock<AppDbContext>();
        _dbContextMock.Setup(x => x.Points).Returns(() => _dbContext.Points);
        _dbContextMock.Setup(x => x.Regions).Returns(() => _dbContext.Regions);
        _dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _dbContextMock.Setup(x => x.AddRangeAsync(It.IsAny<object[]>()))
            .Callback((object[] entities) => _entities = entities)
            .Returns(Task.CompletedTask);
    }
}