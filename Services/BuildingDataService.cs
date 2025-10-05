using Flexlog_api.Models;

namespace Flexlog_api.Services;


public class BuildingDataService
{
    private readonly Dictionary<string, Building> _buildings;
    private readonly Dictionary<string, POIs> _pois;
    private readonly object _lock = new();

    public BuildingDataService()
    {
        _buildings = InitializeBuildings();
        _pois = InitializePOIs();
    }

    /// <summary>
    /// Get all buildings (returns a copy to prevent external modification)
    /// </summary>
    public Dictionary<string, Building> GetBuildings()
    {
        lock (_lock)
        {
            return new Dictionary<string, Building>(_buildings);
        }
    }

    /// <summary>
    /// Get all POIs (returns a copy to prevent external modification)
    /// </summary>
    public Dictionary<string, POIs> GetPOIs()
    {
        lock (_lock)
        {
            return new Dictionary<string, POIs>(_pois);
        }
    }

    /// <summary>
    /// Get a specific building by ID
    /// </summary>
    public Building? GetBuilding(string id)
    {
        lock (_lock)
        {
            return _buildings.TryGetValue(id, out var building) ? building : null;
        }
    }

    /// <summary>
    /// Update a building's state (used by background service)
    /// </summary>
    public void UpdateBuilding(string id, int occupancy, string state)
    {
        lock (_lock)
        {
            if (_buildings.TryGetValue(id, out var building))
            {
                building.Occupancy = occupancy;
                building.State = state;
            }
        }
    }

    public MapData GetMapData()
    {
        lock (_lock)
        {
            return new MapData
            {
                Buildings = GetBuildings(),
                POIs = GetPOIs()
            };
        }
    }

    private Dictionary<string, Building> InitializeBuildings()
    {
        const int SPACING = 80; // Gap between each building
        const int START_X = 100;
        const int START_Y = 100;
        
        var building1 = new Building
        {
            Name = "Adamo tower",
            Coordinates = new Coordinates
            {
                X = START_X,
                Y = START_Y,
                Width = 200,
                Length = 300
            },
            Occupancy = 250,
            State = "occupied",
            Address = "18 Pham Hung Hanoi",
            ImageUrl = null
        };
        var building2 = new Building
        {
            Name = "Flexlog tower",
            Coordinates = new Coordinates
            {
                X = START_X + 200 + (SPACING * 3),
                Y = START_Y + 50,
                Width = 180,
                Length = 250
            },
            Occupancy = 170,
            State = "empty",
            Address = "456 Oak Avenue",
            ImageUrl = null
        };
        var building3 = new Building
        {
            Name = "CMC tower",
            Coordinates = new Coordinates
            {
                X = START_X + 100,
                Y = START_Y + 300 + (SPACING * 2),
                Width = 300,
                Length = 150
            },
            Occupancy = 180,
            State = "repair",
            Address = "789 Pine Road",
            ImageUrl = null
        };
        return new Dictionary<string, Building>
        {
           [building1.Id] = building1,
           [building2.Id] = building2,
           [building3.Id] = building3,
        };
    }
    private Dictionary<string, POIs> InitializePOIs()
    {
        var poi1 = new POIs
        {
            Name = "Tram Station",
            Coordinates = new Coordinates
            {
                X = 200,
                Y = 75
            },
            Type = "transport",
            Description = "Main tram stop for workers"
        };
        var poi2 = new POIs
        {
            Name = "Parking Lot",
            Coordinates = new Coordinates
            {
                X = 450,
                Y = 300
            },
            Type = "parking",
            Description = "200 spaces available"
        };
        var poi3 = new POIs
        {
            Name = "Loading Dock",
            Coordinates = new Coordinates
            {
                X = 100,
                Y = 500
            },
            Type = "logistics",
            Description = "Primary truck loading area"
        };
        return new Dictionary<string, POIs>
        {
            [poi1.Id] = poi1,
            [poi2.Id] = poi2,
            [poi3.Id] = poi3
        };
    }
}