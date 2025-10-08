namespace Flexlog_api.Models;

public class MapData
{
    public Dictionary<string, Building> Buildings { get; set; } = new();
    public Dictionary<string, POIs> POIs { get; set; } = new();
}