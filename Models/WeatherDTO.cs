public class WeatherDTO
{
    public string City { get; set; } = string.Empty;
    public string Country { get; set; }
    public int Temperature { get; set; }
    public string Summary { get; set; } = string.Empty;
    public DateTime RetrievedAt { get; set; } 
    public DateTime ForecastedAt { get; set; }
}