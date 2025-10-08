namespace Flexlog_api.Models;


public class Coordinates
{
    public int X { get; set; }
    public int Y { get; set; }
    
    /// Width of the object (optional, null for POIs)
    public int? Width { get; set; }
    /// Length/Height of the object (optional, null for POIs)
    public int? Length { get; set; }
}