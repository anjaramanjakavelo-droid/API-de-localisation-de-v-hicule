using VehicleTrackingApi.Models;

namespace VehicleTrackingApi.Services;

/// <summary>
/// Détermine le statut de mouvement d'un véhicule à partir de ses positions GPS.
///
/// Logique :
///   - Inactif      → aucune position, ou dernière position > 30 minutes
///   - En mouvement → dernière position ≤ 30 min ET distance entre les 2 derniers points > 50 m
///   - À l'arrêt    → dernière position ≤ 30 min ET distance entre les 2 derniers points ≤ 50 m
///
/// Ces seuils sont adaptés à un contexte pédagogique où les positions
/// sont envoyées manuellement via Postman à des intervalles variables.
/// </summary>
public class StatutService
{
    // Seuil de temps : si la dernière position date de plus de 30 min → inactif
    private const int SeuilInactiviteMinutes = 30;

    // Seuil de distance : moins de 50 m entre deux points → véhicule considéré à l'arrêt
    private const double SeuilMouvementMetres = 50.0;

    private readonly DistanceService _distanceService;

    public StatutService(DistanceService distanceService)
    {
        _distanceService = distanceService;
    }

    /// <summary>
    /// Calcule le statut du véhicule à partir de ses positions triées chronologiquement.
    /// </summary>
    public StatutResult DeterminerStatut(int vehiculeId, IReadOnlyList<PositionGps> positions)
    {
        // Aucune position enregistrée
        if (positions.Count == 0)
        {
            return new StatutResult
            {
                VehiculeId  = vehiculeId,
                Statut      = "Inactif",
                Description = "Aucune position GPS enregistrée."
            };
        }

        var derniere = positions[^1]; // dernière position (liste triée chronologiquement)
        var maintenant = DateTime.Now;
        var minutesDepuisDernierePos = (maintenant - derniere.DatePosition).TotalMinutes;

        // Véhicule inactif depuis trop longtemps
        if (minutesDepuisDernierePos > SeuilInactiviteMinutes)
        {
            return new StatutResult
            {
                VehiculeId               = vehiculeId,
                Statut                   = "Inactif",
                Description              = $"Dernière position reçue il y a {(int)minutesDepuisDernierePos} minute(s).",
                DerniereLat              = derniere.Latitude,
                DerniereLon              = derniere.Longitude,
                DernierePosition         = derniere.DatePosition,
                DistanceDernierSegmentM  = null
            };
        }

        // Un seul point : actif mais impossible de calculer un mouvement
        if (positions.Count == 1)
        {
            return new StatutResult
            {
                VehiculeId       = vehiculeId,
                Statut           = "À l'arrêt",
                Description      = "Un seul point GPS disponible, mouvement indéterminable.",
                DerniereLat      = derniere.Latitude,
                DerniereLon      = derniere.Longitude,
                DernierePosition = derniere.DatePosition
            };
        }

        // Calcul de la distance entre les 2 dernières positions (en mètres)
        var avantDerniere = positions[^2];
        var distanceKm    = _distanceService.CalculerDistanceHaversine(
            (double)avantDerniere.Latitude, (double)avantDerniere.Longitude,
            (double)derniere.Latitude,      (double)derniere.Longitude);
        var distanceMetres = distanceKm * 1000.0;

        if (distanceMetres > SeuilMouvementMetres)
        {
            return new StatutResult
            {
                VehiculeId              = vehiculeId,
                Statut                  = "En mouvement",
                Description             = $"Déplacement de {distanceMetres:F0} m détecté sur le dernier segment.",
                DerniereLat             = derniere.Latitude,
                DerniereLon             = derniere.Longitude,
                DernierePosition        = derniere.DatePosition,
                DistanceDernierSegmentM = Math.Round(distanceMetres, 1)
            };
        }

        return new StatutResult
        {
            VehiculeId              = vehiculeId,
            Statut                  = "À l'arrêt",
            Description             = $"Déplacement de seulement {distanceMetres:F0} m entre les deux dernières positions.",
            DerniereLat             = derniere.Latitude,
            DerniereLon             = derniere.Longitude,
            DernierePosition        = derniere.DatePosition,
            DistanceDernierSegmentM = Math.Round(distanceMetres, 1)
        };
    }
}
