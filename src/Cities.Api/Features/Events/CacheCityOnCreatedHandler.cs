using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

public class CacheCityOnCreatedHandler : INotificationHandler<CityCreated>
{
    private readonly IMemoryCache _cache;
    private readonly CacheOptions _cacheOptions;
    private readonly ICityNormalizer _normalizer;

    public CacheCityOnCreatedHandler(
        IMemoryCache cache,
        IOptions<CacheOptions> cacheOptions,
        ICityNormalizer normalizer)
    {
        _cache = cache;
        _cacheOptions = cacheOptions.Value;
        _normalizer = normalizer;
    }

    public Task Handle(CityCreated notification, CancellationToken cancellationToken)
    {
        var key = CityCacheKey.For(notification.TenantId, _normalizer.Normalize(notification.Name));
        var city = new City
        {
            Id = notification.Id,
            Name = notification.Name,
            Country = notification.Country,
            Latitude = notification.Latitude,
            Longitude = notification.Longitude,
            TimeZone = notification.TimeZone,
            TenantId = notification.TenantId
        };

        _cache.Set(key, city, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheOptions.Duration)
        });

        return Task.CompletedTask;
    }
}
