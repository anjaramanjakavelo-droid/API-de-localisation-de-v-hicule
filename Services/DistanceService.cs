using VehicleTrackingApi.Models;

namespace VehicleTrackingApi.Services;

public class DistanceService
{
    private const double RayonTerreKm = 6371.0;

    /// <summary>
    /// Calcule la distance totale parcourue à partir de positions ordonnées chronologiquement.
    /// Utilise la formule de Haversine entre chaque paire de points successifs.
    /// </summary>
    public double CalculerDistanceTotale(IReadOnlyList<PositionGps> positions)
    {
        if (positions.Count < 2)
            return 0;

        double total = 0;
        for (int i = 1; i < positions.Count; i++)
        {
            total += CalculerDistanceHaversine(
                (double)positions[i - 1].Latitude, (double)positions[i - 1].Longitude,
                (double)positions[i].Latitude,     (double)positions[i].Longitude);
        }

        return Math.Round(total, 2);
    }

    /// <summary>
    /// Calcule la vitesse pour chaque segment entre deux positions successives.
    /// Retourne null si moins de 2 positions disponibles.
    /// </summary>
    public VitesseResult? CalculerVitesses(int vehiculeId, IReadOnlyList<PositionGps> positions)
    {
        if (positions.Count < 2)
            return null;

        var segments = new List<SegmentVitesse>();

        for (int i = 1; i < positions.Count; i++)
        {
            var precedente = positions[i - 1];
            var actuelle   = positions[i];

            var distanceKm  = CalculerDistanceHaversine(
                (double)precedente.Latitude, (double)precedente.Longitude,
                (double)actuelle.Latitude,   (double)actuelle.Longitude);

            var dureeMinutes = (actuelle.DatePosition - precedente.DatePosition).TotalMinutes;
            var dureeHeures  = dureeMinutes / 60.0;

            // On ignore les segments sans écart de temps pour éviter une division par zéro
            var vitesseKmh = dureeHeures > 0 ? Math.Round(distanceKm / dureeHeures, 1) : 0;

            segments.Add(new SegmentVitesse
            {
                DebutSegment = precedente.DatePosition,
                FinSegment   = actuelle.DatePosition,
                DistanceKm   = Math.Round(distanceKm, 3),
                DureeMinutes = Math.Round(dureeMinutes, 1),
                VitesseKmh   = vitesseKmh
            });
        }

        var vitessesValides = segments.Where(s => s.VitesseKmh > 0).ToList();

        return new VitesseResult
        {
            VehiculeId        = vehiculeId,
            VitesseMoyenneKmh = vitessesValides.Count > 0
                                ? Math.Round(vitessesValides.Average(s => s.VitesseKmh), 1)
                                : 0,
            VitesseMaxKmh     = vitessesValides.Count > 0
                                ? vitessesValides.Max(s => s.VitesseKmh)
                                : 0,
            Segments          = segments
        };
    }

    /// <summary>
    /// Calcule la distance en kilomètres entre deux coordonnées GPS (formule de Haversine).
    /// Méthode publique pour être réutilisée par d'autres services (ex: StatutService).
    /// </summary>
    public double CalculerDistanceHaversine(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = DegresVersRadians(lat2 - lat1);
        var dLon = DegresVersRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(DegresVersRadians(lat1)) * Math.Cos(DegresVersRadians(lat2))
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return RayonTerreKm * c;
    }

    private static double DegresVersRadians(double degres) => degres * Math.PI / 180.0;
}
