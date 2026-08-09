public interface IWeatherService
{
    Task<WeatherDTO?> GetCurrentWeatherByCityAsync(string city);
    Task<List<WeatherDTO>?> GetForecastWeatherByCityAsync(string city);
    Task<WeatherDTO> GetWeatherByCityAsyncAPI(string city, CancellationToken cancellationToken = default);
}