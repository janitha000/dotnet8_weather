using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

public class CityService : ICityService
{
    private readonly ICityRepository _cityRepository;
    private readonly IMemoryCache _cache;
    private readonly ICityNormalizer _cityNormalizer;
    private readonly CacheOptions _cacheOptions;
    private readonly ILogger<CityService> _logger;

    public CityService(
        ICityRepository cityRepository,
        IMemoryCache cache,
        ICityNormalizer cityNormalizer,
        IOptions<CacheOptions> cacheOptions,
        ILogger<CityService> logger)
    {
        _cityRepository = cityRepository;
        _cache = cache;
        _cityNormalizer = cityNormalizer;
        _cacheOptions = cacheOptions.Value;
        _logger = logger;
    }

    public async Task<City?> GetCityByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(name)) return null;

        var normalizedName = _cityNormalizer.Normalize(name);
        var cacheKey = $"city:{normalizedName}";

        if (_cache.TryGetValue(cacheKey, out City? cachedCity))
        {
            _logger.LogInformation("City found in cache: {CityName}", cachedCity!.Name);
            return cachedCity;
        }

        _logger.LogInformation("City not found in cache: {CityName}", name);

        var city = await _cityRepository.GetByNameAsync(normalizedName, cancellationToken);

        if (city is not null)
        {
            _cache.Set(cacheKey, city, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheOptions.Duration)
            });
        }

        return city;
    }

    public async Task<City> CreateCityAsync(City city, CancellationToken cancellationToken = default)
    {
        var normalizedName = _cityNormalizer.Normalize(city.Name);
        var isDuplicate = await _cityRepository.ExistsByNameAsync(normalizedName, cancellationToken);

        if (isDuplicate)
        {
            _logger.LogWarning("City already exists: {CityName}", city.Name);
            throw new DuplicateException($"City with name {city.Name} already exists");
        }

        await _cityRepository.AddAsync(city, cancellationToken);
        await _cityRepository.SaveChangesAsync(cancellationToken);

        var cacheKey = $"city:{normalizedName}";
        _cache.Set(cacheKey, city, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheOptions.Duration)
        });

        return city;
    }
}
