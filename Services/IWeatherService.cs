public interface IWeatherService
{
    Task<WeatherDTO?> GetCurrentWeatherByCityAsync(string city);
    Task<List<WeatherDTO>?> GetForecastWeatherByCityAsync(string city);
}