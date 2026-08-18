
public interface ICityService
{
    Task<City?> GetCityByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<City> CreateCityAsync(City city, CancellationToken cancellationToken = default);
}