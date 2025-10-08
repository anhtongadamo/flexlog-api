using Flexlog_api.Dtos.Requests;
using Flexlog_api.Dtos.Responses;
using Flexlog_api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Flexlog_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;
    private readonly ITokenStore _tokenStore;



    public AuthController(ILogger<AuthController> logger, IConfiguration configuration, ITokenStore tokenStore)
    {
        _logger = logger;
        _configuration = configuration;
        _tokenStore = tokenStore;
    }
    
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Username);

        var userName = _configuration.GetSection("Authentication:UserName").Value;
        var password = _configuration.GetSection("Authentication:Password").Value;

        if (request.Username == userName && request.Password == password)
        {
            var token = "token-" + Guid.NewGuid().ToString();
            
            _logger.LogInformation("Login successful for: {Username}", request.Username);
            
            _tokenStore.AddToken(token);

            return Ok(new LoginResponse
            {
                Token = token,
                Success = true,
                Username = request.Username
            });
        }

        _logger.LogWarning("Failed login attempt for: {Username}", request.Username);
        
        return Unauthorized(new LoginResponse
        {
            Success = false,
            Message = "Invalid username or password"
        });
    }
}

