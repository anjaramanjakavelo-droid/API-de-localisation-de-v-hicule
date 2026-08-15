using VehicleTrackingApi.Models;

namespace VehicleTrackingApi.Services;

public class ZoneService
{
    /// <summary>
    /// Zones géographiques prédéfinies correspondant aux principales villes de Madagascar.
    /// Chaque zone est définie par un rectangle GPS (latitude min/max, longitude min/max).
    /// Les limites sont approximatives — cette logique est pédagogique et ne remplace pas un SIG.
    /// </summary>
    private static readonly ZoneGeographique[] Zones =
    [
        new("Antananarivo",  -19.10, -18.80, 47.40, 47.70),
        new("Toamasina",     -18.30, -17.80, 49.20, 49.60),
        new("Mahajanga",     -16.00, -15.60, 46.20, 46.60),
        new("Fianarantsoa",  -21.60, -21.30, 47.00, 47.30),
        new("Antsiranana",   -12.50, -12.15, 49.15, 49.45),
        new("Toliara",       -23.45, -23.20, 43.55, 43.80),
    ];

    /// <summary>
    /// Détermine la ville ou zone géographique d'une position GPS.
    /// Vérifie dans l'ordre si les coordonnées sont contenues dans l'une des zones prédéfinies.
    /// Retourne "Hors zone définie" si aucune correspondance n'est trouvée.
    /// </summary>
    public string DeterminerZone(decimal latitude, decimal longitude)
    {
        var lat = (double)latitude;
        var lon = (double)longitude;

        foreach (var zone in Zones)
        {
            if (lat >= zone.LatMin && lat <= zone.LatMax &&
                lon >= zone.LonMin && lon <= zone.LonMax)
            {
                return zone.Nom;
            }
        }

        return "Hors zone définie";
    }

    /// <summary>
    /// Retourne la liste des zones traversées par un véhicule, sans doublons,
    /// dans l'ordre de première apparition.
    /// </summary>
    public List<string> ObtenirZonesTraversees(IEnumerable<PositionGps> positions)
    {
        return positions
            .Select(p => DeterminerZone(p.Latitude, p.Longitude))
            .Distinct()
            .ToList();
    }

    /// <summary>
    /// Représente une zone géographique définie par un rectangle de coordonnées GPS.
    /// </summary>
    private record ZoneGeographique(
        string Nom,
        double LatMin,
        double LatMax,
        double LonMin,
        double LonMax);
}
