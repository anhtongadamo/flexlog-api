using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Flexlog_api.Services;

public class FlexlogWebSocketManager
{
    private readonly ConcurrentBag<WebSocket> _sockets = new();
    private readonly ILogger<WebSocketManager> _logger;

    public FlexlogWebSocketManager(ILogger<WebSocketManager> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handle a new WebSocket connection
    /// Keeps the connection alive and listens for messages
    /// </summary>
    public async Task HandleWebSocketAsync(WebSocket webSocket)
    {
        _sockets.Add(webSocket);
        _logger.LogInformation("✅ New WebSocket client connected. Total clients: {Count}", 
            _sockets.Count(s => s.State == WebSocketState.Open));

        var buffer = new byte[1024 * 4];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), 
                    CancellationToken.None
                );

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure, 
                        "Closed by client", 
                        CancellationToken.None
                    );
                    _logger.LogInformation("Client requested connection close");
                }
            }
        }
        catch (WebSocketException ex)
        {
            _logger.LogError(ex, "❌ WebSocket error occurred");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Unexpected error in WebSocket handler");
        }
        finally
        {
            _logger.LogInformation("🔌 Client disconnected. Remaining: {Count}", 
                _sockets.Count(s => s.State == WebSocketState.Open));
        }
    }

    /// <summary>
    /// Broadcast a message to all connected WebSocket clients
    /// </summary>
    public async Task BroadcastAsync(object data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        var bytes = Encoding.UTF8.GetBytes(json);

        var activeSockets = _sockets
            .Where(s => s.State == WebSocketState.Open)
            .ToList();

        if (activeSockets.Count == 0)
        {
            _logger.LogWarning("⚠️ No active WebSocket connections to broadcast to");
            return;
        }

        var tasks = activeSockets.Select(async socket =>
        {
            try
            {
                await socket.SendAsync(
                    new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    endOfMessage: true,
                    CancellationToken.None
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending to individual client");
            }
        });

        await Task.WhenAll(tasks);
        _logger.LogInformation("📡 Broadcasted to {Count} clients", activeSockets.Count);
    }
    
    public int GetActiveConnectionCount()
    {
        return _sockets.Count(s => s.State == WebSocketState.Open);
    }
}