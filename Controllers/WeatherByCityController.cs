using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/weather")]
public class CurrentWeatherByCityController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public CurrentWeatherByCityController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("{city}")]
    public async Task<IActionResult> GetCurrentWeatherByCityAsync(string city)
    {
        var weather = await _weatherService.GetCurrentWeatherByCityAsync(city);
        if (weather == null) return NotFound("City not found");
        return Ok(weather);
    }

    [HttpGet("{city}/forecast")]
    public async Task<IActionResult> GetForecastWeatherByCityAsync(string city)
    {
        var weather = await _weatherService.GetForecastWeatherByCityAsync(city);
        if (weather == null) return NotFound("City not found");
        return Ok(weather);
    }
}