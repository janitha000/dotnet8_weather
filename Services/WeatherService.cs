using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;

public class WeatherService : IWeatherService
{

    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

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

    public async Task<WeatherDTO> GetWeatherByCityAsyncAPI(string city, CancellationToken cancellationToken = default)
    {
        var Latitude = "80.6356";
        var Longitude = "7.2955";
        var url =$"v1/forecast?latitude={Latitude}&longitude={Longitude}" +"&current=temperature_2m,weather_code";
        
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to get weather data for {city}");
        }

        var api = await response.Content.ReadFromJsonAsync<OpenMeteoResponse>();
        if (api?.Current is null) return null;
        return new WeatherDTO
        {
            City = city,
            Country = "Sri Lanka",
            Temperature = (int)Math.Round(api.Current.Temperature_2m),
            Summary = $"Code {api.Current.Weather_Code}",
            RetrievedAt = DateTime.UtcNow
        };
    }
}