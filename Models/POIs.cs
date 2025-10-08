namespace Flexlog_api.Models;


public class POIs
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// POI name (e.g., "Tram Station")
    public string Name { get; set; } = string.Empty;
    public Coordinates Coordinates { get; set; } = new();
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
}