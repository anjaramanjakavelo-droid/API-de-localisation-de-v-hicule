namespace VehicleTrackingApi.Models;

public class ZonesResult
{
    public int VehiculeId { get; set; }
    public List<string> Zones { get; set; } = new();
}
