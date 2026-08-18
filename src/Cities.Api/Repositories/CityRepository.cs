using Microsoft.EntityFrameworkCore;

public interface ICityRepository
{
    Task<City?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string normalizedName, CancellationToken cancellationToken = default);
    Task AddAsync(City city, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class CityRepository : ICityRepository
{
    private readonly AppDbContext _dbContext;

    public CityRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<City?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        // `name` is expected to be normalized (trimmed + lowercase) by the service
        return await _dbContext.Cities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name.ToLower() == name, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cities
            .AnyAsync(c => c.Name.ToLower() == normalizedName, cancellationToken);
    }

    public async Task AddAsync(City city, CancellationToken cancellationToken = default)
    {
        await _dbContext.Cities.AddAsync(city, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
