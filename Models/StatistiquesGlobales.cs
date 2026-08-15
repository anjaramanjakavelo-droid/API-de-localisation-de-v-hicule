namespace VehicleTrackingApi.Models;

/// <summary>
/// Résumé agrégé de l'ensemble du parc de véhicules.
/// Permet au client de récupérer toutes les métriques en un seul appel.
/// </summary>
public class StatistiquesGlobales
{
    public int      NbVehicules        { get; set; }
    public int      NbPositionsTotal   { get; set; }
    public double   DistanceTotaleKm   { get; set; }
    public DateTime? DerniereActivite  { get; set; }
    public string?  DernierVehicule    { get; set; }
}
