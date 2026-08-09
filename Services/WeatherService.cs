public class WeatherService : IWeatherService
{
    public Task<WeatherDTO?> GetCurrentWeatherByCityAsync(string city)
    {
        if (string.IsNullOrEmpty(city)) return Task.FromResult<WeatherDTO?>(null);
    

        var result = new WeatherDTO
        {
            City = city,
            Country = "United States",
            Temperature = Random.Shared.Next(-5, 35),
            Summary = "Sunny",
            RetrievedAt = DateTime.UtcNow
        };

        return Task.FromResult<WeatherDTO?>(result);
    }

    public Task<List<WeatherDTO>?> GetForecastWeatherByCityAsync(string city)
    {
        if (string.IsNullOrEmpty(city)) return Task.FromResult<List<WeatherDTO>?>(null);

        var result = new List<WeatherDTO>
        {
            new WeatherDTO { City = city, Country = "United States", Temperature = Random.Shared.Next(-5, 35), Summary = "Sunny", RetrievedAt = DateTime.UtcNow, ForecastedAt = DateTime.UtcNow.AddDays(1) },
            new WeatherDTO { City = city, Country = "United States", Temperature = Random.Shared.Next(-5, 35), Summary = "Sunny", RetrievedAt = DateTime.UtcNow, ForecastedAt = DateTime.UtcNow.AddDays(2) },
            new WeatherDTO { City = city, Country = "United States", Temperature = Random.Shared.Next(-5, 35), Summary = "Sunny", RetrievedAt = DateTime.UtcNow, ForecastedAt = DateTime.UtcNow.AddDays(3) },
            new WeatherDTO { City = city, Country = "United States", Temperature = Random.Shared.Next(-5, 35), Summary = "Sunny", RetrievedAt = DateTime.UtcNow, ForecastedAt = DateTime.UtcNow.AddDays(4) },
            new WeatherDTO { City = city, Country = "United States", Temperature = Random.Shared.Next(-5, 35), Summary = "Sunny", RetrievedAt = DateTime.UtcNow, ForecastedAt = DateTime.UtcNow.AddDays(5) },
            new WeatherDTO { City = city, Country = "United States", Temperature = Random.Shared.Next(-5, 35), Summary = "Sunny", RetrievedAt = DateTime.UtcNow, ForecastedAt = DateTime.UtcNow.AddDays(6) },
        };

        return Task.FromResult<List<WeatherDTO>?>(result);
    }
}