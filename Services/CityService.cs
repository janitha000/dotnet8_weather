using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;



public class CityService : ICityService
{
    private readonly AppDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private readonly ICityNormalizer _cityNormalizer;
    private readonly CacheOptions _cacheOptions;
    private readonly ILogger<CityService> _logger;

    public CityService(
        AppDbContext dbContext, 
        IMemoryCache cache, 
        ICityNormalizer cityNormalizer,
         IOptions<CacheOptions> cacheOptions, 
         ILogger<CityService> logger)
    {
        _dbContext = dbContext;
        _cache = cache;
        _cityNormalizer = cityNormalizer;
        _cacheOptions = cacheOptions.Value;
        _logger = logger;
    }

    public async Task<City?> GetCityByNameAsync(string name)
    {
        if(string.IsNullOrEmpty(name)) return null;

        var cacheKey = $"city:{_cityNormalizer.Normalize(name)}";

        if (_cache.TryGetValue(cacheKey, out City? cachedCity)){
            _logger.LogInformation("City found in cache: {CityName}", cachedCity.Name);
            return cachedCity;

        }

        _logger.LogInformation("City not found in cache: {CityName}", name);

        var city = await _dbContext.Cities
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

        if(city is not null){
            _cache.Set(cacheKey, city, new MemoryCacheEntryOptions{
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheOptions.Duration)
            });
        }

        return city;
    } 

    public async Task<City> CreateCityAsync(City city){
        var normalizedName = _cityNormalizer.Normalize(city.Name);
        var isDuplicate = await _dbContext.Cities.AnyAsync(c => c.Name.ToLower() == normalizedName.ToLower());
        
        if(isDuplicate) {
            _logger.LogWarning("City already exists: {CityName}", city.Name);
            throw new DuplicateException($"City with name {city.Name} already exists");
        }


        _dbContext.Cities.Add(city);
        await _dbContext.SaveChangesAsync();

        var cacheKey = $"city:{_cityNormalizer.Normalize(city.Name)}";
         _cache.Set(cacheKey, city, new MemoryCacheEntryOptions{
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheOptions.Duration)
            });
        return city;
    }
}