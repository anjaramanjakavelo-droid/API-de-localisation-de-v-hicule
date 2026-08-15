namespace VehicleTrackingApi.Models;

/// <summary>
/// Statut de mouvement d'un véhicule déduit de ses dernières positions GPS.
/// </summary>
public class StatutResult
{
    public int     VehiculeId        { get; set; }
    public string  Statut            { get; set; } = string.Empty; // "En mouvement" | "À l'arrêt" | "Inactif"
    public string  Description       { get; set; } = string.Empty;
    public decimal? DerniereLat      { get; set; }
    public decimal? DerniereLon      { get; set; }
    public DateTime? DernierePosition { get; set; }
    public double?  DistanceDernierSegmentM { get; set; } // distance en mètres entre les 2 derniers points
}
