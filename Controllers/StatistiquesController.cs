using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleTrackingApi.Data;
using VehicleTrackingApi.Models;
using VehicleTrackingApi.Services;

namespace VehicleTrackingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatistiquesController : ControllerBase
{
    private readonly AppDbContext    _context;
    private readonly DistanceService _distanceService;

    public StatistiquesController(AppDbContext context, DistanceService distanceService)
    {
        _context         = context;
        _distanceService = distanceService;
    }

    /// <summary>
    /// Retourne un résumé agrégé de l'ensemble du parc en un seul appel.
    /// Évite les N+1 requêtes côté client.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<StatistiquesGlobales>> GetStatistiques()
    {
        var vehicules  = await _context.Vehicules.AsNoTracking().ToListAsync();
        var positions  = await _context.PositionsGps.AsNoTracking()
                             .OrderBy(p => p.DatePosition)
                             .ToListAsync();

        // Distance totale : somme des distances par véhicule
        double distanceTotale = 0;
        foreach (var v in vehicules)
        {
            var posVehicule = positions
                .Where(p => p.VehiculeId == v.Id)
                .ToList();
            distanceTotale += _distanceService.CalculerDistanceTotale(posVehicule);
        }

        // Dernière activité toutes flottes confondues
        var derniere = positions.OrderByDescending(p => p.DatePosition).FirstOrDefault();
        string? dernierVehicule = null;
        if (derniere is not null)
        {
            var veh = vehicules.FirstOrDefault(v => v.Id == derniere.VehiculeId);
            dernierVehicule = veh?.Immatriculation ?? $"#{derniere.VehiculeId}";
        }

        return Ok(new StatistiquesGlobales
        {
            NbVehicules       = vehicules.Count,
            NbPositionsTotal  = positions.Count,
            DistanceTotaleKm  = Math.Round(distanceTotale, 2),
            DerniereActivite  = derniere?.DatePosition,
            DernierVehicule   = dernierVehicule
        });
    }
}
