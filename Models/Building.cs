namespace Flexlog_api.Models;

/// <summary>
/// Represents a building on the map with dynamic state
/// </summary>
public class Building
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public Coordinates Coordinates { get; set; } = new();
    public int Occupancy { get; set; }
    public string State { get; set; } = "empty";
    public string Address { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}