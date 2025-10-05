using Microsoft.AspNetCore.Mvc;

namespace Flexlog_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }
    
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Username);
        
        if (request.Username == "admin" && request.Password == "password")
        {
            var token = "token-" + Guid.NewGuid().ToString();
            
            _logger.LogInformation("Login successful for: {Username}", request.Username);
            
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


public record LoginRequest(string Username, string Password);

public class LoginResponse
{
    public string? Token { get; set; }
    public bool Success { get; set; }
    public string? Username { get; set; }
    public string? Message { get; set; }
}