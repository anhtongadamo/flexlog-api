using System.Text.Json;
using Flexlog_api.Services;
using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// ====================================
// REGISTER SERVICES (Dependency Injection)
// ====================================

// Add controllers support
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure JSON serialization
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true; // Pretty print for development
    });

// Add API Explorer (for Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSingleton<BuildingDataService>();

// Register WebSocket services
builder.Services.AddSingleton<FlexlogWebSocketManager>();
builder.Services.AddHostedService<BuildingUpdateService>();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// ====================================
// CONFIGURE MIDDLEWARE PIPELINE
// ====================================

// Enable Swagger UI (API documentation) in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseWebSockets();

// ====================================
// WEBSOCKET ENDPOINT
// ====================================

app.Map("/ws", async (HttpContext context, FlexlogWebSocketManager wsManager) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await wsManager.HandleWebSocketAsync(webSocket);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

// Map controller endpoints
app.MapControllers();

// ====================================
// START SERVER
// ====================================

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://localhost:{port}");

app.Logger.LogInformation("==============================================");
app.Logger.LogInformation("Flexlog Backend Server Starting");
app.Logger.LogInformation("==============================================");
app.Logger.LogInformation("HTTP API: http://localhost:{Port}", port);
app.Logger.LogInformation("Swagger UI: http://localhost:{Port}/swagger", port);
app.Logger.LogInformation("==============================================");

app.Run();