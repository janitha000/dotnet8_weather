
public interface ICityService
{
    Task<City?> GetCityByNameAsync(string name);
    Task<City> CreateCityAsync(City city);
}