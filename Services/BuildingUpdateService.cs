using Flexlog_api.Models;

namespace Flexlog_api.Services;

/// <summary>
/// Background service that updates building states every 5 seconds
/// and broadcasts changes to all connected WebSocket clients
/// </summary>
public class BuildingUpdateService : BackgroundService
{
    private readonly FlexlogWebSocketManager _wsManager;
    private readonly ILogger<BuildingUpdateService> _logger;
    private readonly BuildingDataService _buildingDataService;
    private readonly Random _random = new();

    public BuildingUpdateService(
        FlexlogWebSocketManager wsManager,
        ILogger<BuildingUpdateService> logger,
        BuildingDataService buildingDataService
    )
    {
        _wsManager = wsManager;
        _logger = logger;
        _buildingDataService = buildingDataService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 Building State Update Service started");

        // Wait 2 seconds before first update
        await Task.Delay(2000, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                //Get current building
                var buildings = _buildingDataService.GetBuildings();
                // Update building states randomly
                foreach (var keyValues in buildings)
                {
                    var building = keyValues.Value;
                    var newOccupancy  = _random.Next(160, 301);
                    var newState  = _random.Next(3) switch
                    {
                        0 => "empty",
                        1 => "occupied",
                        2 => "repair",
                        _ => "empty"
                    };
                 _buildingDataService.UpdateBuilding(building.Id, newOccupancy, newState);
                }
                // Get updated buildings and broadcast via WebSocket
                var updatedBuildings = _buildingDataService.GetBuildings();
                // Broadcast to all WebSocket clients
                await _wsManager.BroadcastAsync(new
                {
                    type = "buildingUpdate",
                    timestamp = DateTime.UtcNow,
                    buildings = updatedBuildings
                });

                _logger.LogInformation("🔄 Building states updated and broadcasted to {Count} clients", 
                    _wsManager.GetActiveConnectionCount());

                // Wait 5 seconds before next update
                await Task.Delay(5000, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("⏸️ Update service cancelled (app shutting down)");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error updating building states");
                await Task.Delay(5000, stoppingToken); // Wait before retry
            }
        }

        _logger.LogInformation("⏹️ Building State Update Service stopped");
    }
}