using Flexlog_api.Models;
using Flexlog_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Flexlog_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MapController : ControllerBase
{
    private readonly ILogger<MapController> _logger;
    private readonly BuildingDataService _buildingDataService;

    public MapController(
        ILogger<MapController> logger,
        BuildingDataService buildingDataService)
    {
        _logger = logger;
        _buildingDataService = buildingDataService;
    }

    /// <summary>
    /// GET /api/map/data
    /// Returns current map data (buildings and POIs)
    /// </summary>
    [HttpGet("data")]
    public IActionResult GetMapData()
    {
        _logger.LogInformation("Map data requested");

        var mapData = _buildingDataService.GetMapData();

        return Ok(mapData);
    }
}