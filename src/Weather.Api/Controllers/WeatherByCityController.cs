using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/weather")]
public class WeatherByCityController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherByCityController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("{city}")]
    public async Task<IActionResult> GetCurrentWeatherByCityAsync(
        string city,
        CancellationToken cancellationToken)
    {
        var weather = await _weatherService.GetCurrentWeatherByCityAsync(city, cancellationToken);
        if (weather == null) return NotFound("City not found");
        return Ok(weather);
    }

    [HttpGet("{city}/forecast")]
    public async Task<IActionResult> GetForecastWeatherByCityAsync(
        string city,
        CancellationToken cancellationToken)
    {
        var forecast = await _weatherService.GetForecastWeatherByCityAsync(city, cancellationToken);
        if (forecast == null) return NotFound("City not found");
        return Ok(forecast);
    }
}
