using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, City>
{
    private readonly ICityRepository _cityRepository;
    private readonly ICityNormalizer _cityNormalizer;
    private readonly IMemoryCache _memoryCache;
    private readonly CacheOptions _cacheOptions;
    private readonly ILogger<CreateCityCommandHandler> _logger;

    public CreateCityCommandHandler(
        ICityRepository cityRepository,
        ICityNormalizer cityNormalizer,
        IMemoryCache memoryCache,
        IOptions<CacheOptions> cacheOptions,
        ILogger<CreateCityCommandHandler> logger)
    {
        _cityRepository = cityRepository;
        _cityNormalizer = cityNormalizer;
        _memoryCache = memoryCache;
        _cacheOptions = cacheOptions.Value;
        _logger = logger;
    }

    public async Task<City> Handle(CreateCityCommand request, CancellationToken cancellationToken)
    {
        var normalizedName = _cityNormalizer.Normalize(request.Name);
        if (await _cityRepository.ExistsByNameAsync(normalizedName, cancellationToken))
        {
            _logger.LogWarning("Duplicate city create attempt {CityName}", request.Name);
            throw new DuplicateException($"City with name '{request.Name}' already exists");
        }

        var city = new City
        {
            Name = request.Name,
            Country = request.Country,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            TimeZone = request.TimeZone
        };

        await _cityRepository.AddAsync(city, cancellationToken);
        await _cityRepository.SaveChangesAsync(cancellationToken);

        var cacheKey = $"city:{normalizedName}";
        _memoryCache.Set(cacheKey, city, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheOptions.Duration)
        });

        return city;
    }
}
