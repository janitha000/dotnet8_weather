public class CityLookupFromRepository : ICityLookup
{
    private readonly ICityRepository _cities;
    private readonly ICityNormalizer _normalizer;

    public CityLookupFromRepository(ICityRepository cities, ICityNormalizer normalizer)
    {
        _cities = cities;
        _normalizer = normalizer;
    }

    public async Task<CityLocation?> FindByNameAsync(
        string cityName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cityName))
            return null;

        var city = await _cities.GetByNameAsync(
            _normalizer.Normalize(cityName),
            cancellationToken);

        if (city is null)
            return null;

        return new CityLocation
        {
            Name = city.Name,
            Country = city.Country,
            Latitude = city.Latitude,
            Longitude = city.Longitude
        };
    }
}
