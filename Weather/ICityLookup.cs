public sealed class CityLocation
{
    public string Name { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string Latitude { get; init; } = string.Empty;
    public string Longitude { get; init; } = string.Empty;
}

public interface ICityLookup
{
    Task<CityLocation?> FindByNameAsync(string cityName, CancellationToken cancellationToken = default);
}
