namespace VehicleTrackingApi.Models;

/// <summary>
/// Dernière position GPS connue d'un véhicule.
/// </summary>
public class PositionActuelleResult
{
    public int    VehiculeId      { get; set; }
    public string Immatriculation { get; set; } = string.Empty;
    public decimal Latitude       { get; set; }
    public decimal Longitude      { get; set; }
    public DateTime DatePosition  { get; set; }
}
