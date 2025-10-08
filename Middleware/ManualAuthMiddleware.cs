using Flexlog_api.Interfaces;

namespace Flexlog_api.Middleware;

public class ManualAuthMiddleware
{
    private readonly RequestDelegate _next;

    public ManualAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ITokenStore tokenStore)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token) || !tokenStore.IsValid(token))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        await _next(context);
    }
}
