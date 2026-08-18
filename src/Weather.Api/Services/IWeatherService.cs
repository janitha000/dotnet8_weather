public interface IWeatherService
{
    Task<WeatherDTO?> GetCurrentWeatherByCityAsync(string city, CancellationToken cancellationToken = default);
    Task<List<WeatherDTO>?> GetForecastWeatherByCityAsync(string city, CancellationToken cancellationToken = default);
}
