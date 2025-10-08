using Flexlog_api.Interfaces;

namespace Flexlog_api.Services.Authentication;

public class TokenStore : ITokenStore
{
    private readonly HashSet<string> _validTokens = new();

    public void AddToken(string token) => _validTokens.Add(token);
    public bool IsValid(string token) => _validTokens.Contains(token);
}
