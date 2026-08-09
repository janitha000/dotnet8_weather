using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetCityByNameAsync(string name)
    {
        var city = await _cityService.GetCityByNameAsync(name);
        if (city == null) return NotFound("City not found");
        return Ok(city);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(City), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<City>> CreateCityAsync([FromBody] CreateCityDto createCityDto)
    {
        var city = new City
        {
            Name = createCityDto.Name,
            Country = createCityDto.Country,
            Latitude = createCityDto.Latitude,
            Longitude = createCityDto.Longitude
        };

        city = await _cityService.CreateCityAsync(city);
        return CreatedAtAction(nameof(GetCityByNameAsync), new { name = city.Name }, city);
    }
}