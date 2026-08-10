public class WeatherAPIOptions
{
    public const string SectionName = "WeatherApi";
    public string BaseUrl { get; set; } = string.Empty;
}

public class CitiesApiOptions
{
    public const string SectionName = "CitiesApi";

    /// <summary>Http or Grpc — both adapters stay registered; this picks ICityLookup.</summary>
    public string LookupMode { get; set; } = "Http";

    public string BaseUrl { get; set; } = string.Empty;
    public string GrpcUrl { get; set; } = string.Empty;
}
