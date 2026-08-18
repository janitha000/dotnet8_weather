public class CityServiceTests
{
    private static CityService CreateSut(AppDbContext db)
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var normalizer = new CityNormalizer();
        var options = Options.Create(new CacheOptions { Duration = 10 });
        var logger = new Mock<ILogger<CityService>>().Object;
        var repo = new CityRepository(db);

        return new CityService(repo, cache, normalizer, options, logger);
    }

    private static AppDbContext CreateDb(string tenantId = "acme")
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
            .Options;
        return new AppDbContext(opts, new FakeTenantContext(tenantId));
    }

    private static City Colombo(string tenantId = "acme") => new()
    {
        Name = "Colombo",
        Country = "Sri Lanka",
        Latitude = "1",
        Longitude = "2",
        TenantId = tenantId
    };

    [Fact]
    public async Task GetCityByNameAsync_ReturnsCity_WhenExists()
    {
        // Arrange
        await using var db = CreateDb();
        db.Cities.Add(Colombo());
        await db.SaveChangesAsync();
        var sut = CreateSut(db);

        // Act
        var result = await sut.GetCityByNameAsync("Colombo");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Colombo", result!.Name);
    }

    [Fact]
    public async Task CreateCityAsync_ThrowsDuplicateException_WhenNameExists()
    {
        await using var db = CreateDb();
        db.Cities.Add(Colombo());
        await db.SaveChangesAsync();
        var sut = CreateSut(db);

        var act = () => sut.CreateCityAsync(Colombo());

        await Assert.ThrowsAsync<DuplicateException>(act);
    }

    [Fact]
    public async Task GetCityByNameAsync_UsesCache_OnSecondCall()
    {
        await using var db = CreateDb();
        db.Cities.Add(Colombo());
        await db.SaveChangesAsync();
        var sut = CreateSut(db);

        await sut.GetCityByNameAsync("Colombo");
        db.Cities.RemoveRange(db.Cities); // wipe DB
        await db.SaveChangesAsync();

        var result = await sut.GetCityByNameAsync("Colombo"); // should still hit cache

        Assert.NotNull(result);
        Assert.Equal("Colombo", result!.Name);
    }
}