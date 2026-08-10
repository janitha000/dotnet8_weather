using System.Text.Json.Serialization;

public class WeatherDTO
{
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int Temperature { get; set; }
    public string Summary { get; set; } = string.Empty;
    public DateTime RetrievedAt { get; set; }
    public DateTime ForecastedAt { get; set; }
}

public class OpenMeteoResponse
{
    [JsonPropertyName("current")]
    public CurrentWeather? Current { get; set; }

    [JsonPropertyName("daily")]
    public DailyWeather? Daily { get; set; }
}

public class CurrentWeather
{
    [JsonPropertyName("temperature_2m")]
    public double Temperature2m { get; set; }

    [JsonPropertyName("weather_code")]
    public int WeatherCode { get; set; }
}

public class DailyWeather
{
    [JsonPropertyName("time")]
    public List<string> Time { get; set; } = new();

    [JsonPropertyName("temperature_2m_max")]
    public List<double> Temperature2mMax { get; set; } = new();

    [JsonPropertyName("weather_code")]
    public List<int> WeatherCode { get; set; } = new();
}
