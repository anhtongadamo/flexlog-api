namespace Flexlog_api.Interfaces;

public interface ITokenStore
{
    void AddToken(string token);
    bool IsValid(string token);
}
