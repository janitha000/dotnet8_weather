public class WeatherDTO
{
    public string City { get; set; } = string.Empty;
    public string Country { get; set; }
    public int Temperature { get; set; }
    public string Summary { get; set; } = string.Empty;
    public DateTime RetrievedAt { get; set; } 
    public DateTime ForecastedAt { get; set; }
}


public class OpenMeteoResponse
{
    public CurrentWeather? Current { get; set; }
}

public class CurrentWeather
{
    public double Temperature_2m { get; set; }
    public int Weather_Code { get; set; }
}