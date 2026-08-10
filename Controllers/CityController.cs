using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ServiceFilter(typeof(LogActionFilter))]
[ApiController]
[Route("api/cities")]
public class CityController : ControllerBase
{
    private readonly ICityService _cityService;

    public CityController(ICityService cityService)
    {
        _cityService = cityService;
    }

    [HttpGet("{name}")]
    // [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetCityByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var city = await _cityService.GetCityByNameAsync(name, cancellationToken);
        if (city == null) return NotFound("City not found");
        return Ok(city);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(City), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<City>> CreateCityAsync([FromBody] CreateCityDto createCityDto, CancellationToken cancellationToken = default)
    {
        var city = new City
        {
            Name = createCityDto.Name,
            Country = createCityDto.Country,
            Latitude = createCityDto.Latitude,
            Longitude = createCityDto.Longitude
        };

        city = await _cityService.CreateCityAsync(city, cancellationToken);
        return CreatedAtAction(nameof(GetCityByNameAsync), new { name = city.Name }, city);
    }
}