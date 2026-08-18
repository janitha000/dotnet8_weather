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
        var user = DemoUsers.Find(loginDto.Username, loginDto.Password);
        if (user is null)
            return Unauthorized();
        
        var accessToken = _jwtService.GenerateToken(user.Username, user.TenantId, user.Role);
        return Ok(new { accessToken, tenantId = user.TenantId });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me([FromServices] ITenantContext tenant)
    {
        return Ok(new
        {
            username = User.Identity?.Name,
            tenantId = tenant.TenantId,
            role = User.FindFirst(ClaimTypes.Role)?.Value
        });
    }
}
