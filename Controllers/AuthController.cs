using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IJWTService _jwtService;

    public AuthController(ILogger<AuthController> logger, IJWTService jwtService)
    {
        _logger = logger;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        if (loginDto.Username != "admin" || loginDto.Password != "password")
            return Unauthorized();

        var accessToken = _jwtService.GenerateToken(loginDto.Username);
        return Ok(new { accessToken });
    }
}
