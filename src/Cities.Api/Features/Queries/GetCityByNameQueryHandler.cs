using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

public class GetCityByNameQueryHandler : IRequestHandler<GetCityByNameQuery, City?>
{
    private readonly ICityRepository _cityRepository;
    private readonly ICityNormalizer _cityNormalizer;
    private readonly IMemoryCache _memoryCache;
    private readonly CacheOptions _cacheOptions;
    private readonly ILogger<GetCityByNameQueryHandler> _logger;
    private readonly ITenantContext _tenant;

    public GetCityByNameQueryHandler(
        ICityRepository cityRepository,
        ICityNormalizer cityNormalizer,
        IMemoryCache memoryCache,
        IOptions<CacheOptions> cacheOptions,
        ILogger<GetCityByNameQueryHandler> logger,
        ITenantContext tenant)
    {
        _cityRepository = cityRepository;
        _cityNormalizer = cityNormalizer;
        _memoryCache = memoryCache;
        _cacheOptions = cacheOptions.Value;
        _logger = logger;
        _tenant = tenant;
    }

    public async Task<City?> Handle(GetCityByNameQuery request, CancellationToken cancellationToken)
    {
        if (!_tenant.IsResolved)
            return null;

        var normalizedName = _cityNormalizer.Normalize(request.Name);
        var cacheKey = CityCacheKey.For(_tenant.TenantId!, normalizedName);


        if (_memoryCache.TryGetValue(cacheKey, out City? city))
        {
            _logger.LogInformation("City found in cache: {CityName}", city!.Name);
            return city;
        }

        _logger.LogInformation("City not found in cache: {CityName}", request.Name);
        city = await _cityRepository.GetByNameAsync(normalizedName, cancellationToken);

        if (city is not null)
        {
            _memoryCache.Set(cacheKey, city, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheOptions.Duration)
            });
        }

        return city;
    }
}
