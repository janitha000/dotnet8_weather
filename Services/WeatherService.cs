using System.Net.Http.Json;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ICityRepository _cityRepository;
    private readonly ICityNormalizer _cityNormalizer;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(
        HttpClient httpClient,
        ICityRepository cityRepository,
        ICityNormalizer cityNormalizer,
        ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _cityRepository = cityRepository;
        _cityNormalizer = cityNormalizer;
        _logger = logger;
    }

    public async Task<WeatherDTO?> GetCurrentWeatherByCityAsync(
        string city,
        CancellationToken cancellationToken = default)
    {
        var entity = await FindCityAsync(city, cancellationToken);
        if (entity is null) return null;

        var url =
            $"v1/forecast?latitude={entity.Latitude}&longitude={entity.Longitude}" +
            "&current=temperature_2m,weather_code";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Weather API failed with {StatusCode} for {City}",
                response.StatusCode,
                entity.Name);
            throw new AppException(
                StatusCodes.Status502BadGateway,
                $"Failed to get current weather for '{entity.Name}'");
        }

        var api = await response.Content.ReadFromJsonAsync<OpenMeteoResponse>(cancellationToken: cancellationToken);
        if (api?.Current is null) return null;

        return new WeatherDTO
        {
            City = entity.Name,
            Country = entity.Country,
            Temperature = (int)Math.Round(api.Current.Temperature2m),
            Summary = WeatherCodeToSummary(api.Current.WeatherCode),
            RetrievedAt = DateTime.UtcNow
        };
    }

    public async Task<List<WeatherDTO>?> GetForecastWeatherByCityAsync(
        string city,
        CancellationToken cancellationToken = default)
    {
        var entity = await FindCityAsync(city, cancellationToken);
        if (entity is null) return null;

        var url =
            $"v1/forecast?latitude={entity.Latitude}&longitude={entity.Longitude}" +
            "&daily=temperature_2m_max,weather_code&timezone=auto&forecast_days=7";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Weather forecast API failed with {StatusCode} for {City}",
                response.StatusCode,
                entity.Name);
            throw new AppException(
                StatusCodes.Status502BadGateway,
                $"Failed to get forecast for '{entity.Name}'");
        }

        var api = await response.Content.ReadFromJsonAsync<OpenMeteoResponse>(cancellationToken: cancellationToken);
        if (api?.Daily?.Time is null || api.Daily.Time.Count == 0) return null;

        var results = new List<WeatherDTO>();
        for (var i = 0; i < api.Daily.Time.Count; i++)
        {
            var temp = i < api.Daily.Temperature2mMax.Count
                ? (int)Math.Round(api.Daily.Temperature2mMax[i])
                : 0;
            var code = i < api.Daily.WeatherCode.Count ? api.Daily.WeatherCode[i] : 0;
            var day = DateTime.TryParse(api.Daily.Time[i], out var parsed)
                ? parsed
                : DateTime.UtcNow.Date.AddDays(i);

            results.Add(new WeatherDTO
            {
                City = entity.Name,
                Country = entity.Country,
                Temperature = temp,
                Summary = WeatherCodeToSummary(code),
                RetrievedAt = DateTime.UtcNow,
                ForecastedAt = day
            });
        }

        return results;
    }

    private async Task<City?> FindCityAsync(string city, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(city)) return null;

        var normalized = _cityNormalizer.Normalize(city);
        return await _cityRepository.GetByNameAsync(normalized, cancellationToken);
    }

    private static string WeatherCodeToSummary(int code) => code switch
    {
        0 => "Clear",
        1 or 2 or 3 => "Partly cloudy",
        45 or 48 => "Fog",
        51 or 53 or 55 => "Drizzle",
        61 or 63 or 65 => "Rain",
        71 or 73 or 75 => "Snow",
        80 or 81 or 82 => "Rain showers",
        95 => "Thunderstorm",
        _ => $"Code {code}"
    };
}
