namespace VehicleTrackingApi.Models;

/// <summary>
/// Résultat du calcul de vitesse pour un véhicule.
/// </summary>
public class VitesseResult
{
    public int    VehiculeId        { get; set; }
    public double VitesseMoyenneKmh { get; set; }
    public double VitesseMaxKmh     { get; set; }

    /// <summary>Vitesse calculée entre chaque paire de positions successives.</summary>
    public List<SegmentVitesse> Segments { get; set; } = new();
}

/// <summary>
/// Vitesse calculée entre deux positions GPS consécutives.
/// </summary>
public class SegmentVitesse
{
    public DateTime DebutSegment  { get; set; }
    public DateTime FinSegment    { get; set; }
    public double   DistanceKm    { get; set; }
    public double   DureeMinutes  { get; set; }
    public double   VitesseKmh    { get; set; }
}
