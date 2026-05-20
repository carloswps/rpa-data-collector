using Microsoft.AspNetCore.Mvc;
using rpa_data_collector.Application.Services;
using rpa_data_collector.DTOs;

namespace rpa_data_collector.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly TokenService _tokenService;

    public AuthController(IConfiguration configuration, TokenService tokenService)
    {
        _configuration = configuration;
        _tokenService = tokenService;
    }

    [HttpPost("authenticate")]
    public IActionResult Login([FromBody] LoginRequestDto loginRequest)
    {
        var userName = _configuration["Auth:UserName"];
        var password = _configuration["Auth:Password"];

        if (loginRequest.Username != userName || loginRequest.Password != password) return Unauthorized();

        var token = _tokenService.GenerateToken(userName, password);
        return Ok(new { token });
    }
}
