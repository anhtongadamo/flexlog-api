namespace Flexlog_api.Dtos.Responses;

public class LoginResponse
{
    public string? Token { get; set; }
    public bool Success { get; set; }
    public string? Username { get; set; }
    public string? Message { get; set; }
}