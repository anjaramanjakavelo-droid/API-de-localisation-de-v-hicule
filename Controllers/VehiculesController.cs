using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleTrackingApi.Data;
using VehicleTrackingApi.Models;
using VehicleTrackingApi.Services;

namespace VehicleTrackingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiculesController : ControllerBase
{
    private readonly AppDbContext    _context;
    private readonly DistanceService _distanceService;
    private readonly ZoneService     _zoneService;
    private readonly StatutService   _statutService;

    public VehiculesController(
        AppDbContext context,
        DistanceService distanceService,
        ZoneService zoneService,
        StatutService statutService)
    {
        _context         = context;
        _distanceService = distanceService;
        _zoneService     = zoneService;
        _statutService   = statutService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehicule>>> GetAll()
    {
        var vehicules = await _context.Vehicules
            .AsNoTracking()
            .OrderBy(v => v.Immatriculation)
            .ToListAsync();

        return Ok(vehicules);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Vehicule>> GetById(int id)
    {
        var vehicule = await _context.Vehicules
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicule is null)
            return NotFound(new { message = $"Véhicule {id} introuvable." });

        return Ok(vehicule);
    }

    [HttpPost]
    public async Task<ActionResult<Vehicule>> Create([FromBody] Vehicule vehicule)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existe = await _context.Vehicules
            .AnyAsync(v => v.Immatriculation == vehicule.Immatriculation);

        if (existe)
            return BadRequest(new { message = "Cette immatriculation existe déjà." });

        _context.Vehicules.Add(vehicule);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = vehicule.Id }, vehicule);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Vehicule vehicule)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existant = await _context.Vehicules.FindAsync(id);
        if (existant is null)
            return NotFound(new { message = $"Véhicule {id} introuvable." });

        var immatriculationPrise = await _context.Vehicules
            .AnyAsync(v => v.Immatriculation == vehicule.Immatriculation && v.Id != id);

        if (immatriculationPrise)
            return BadRequest(new { message = "Cette immatriculation existe déjà." });

        existant.Immatriculation = vehicule.Immatriculation;
        await _context.SaveChangesAsync();

        return Ok(existant);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var vehicule = await _context.Vehicules.FindAsync(id);
        if (vehicule is null)
            return NotFound(new { message = $"Véhicule {id} introuvable." });

        _context.Vehicules.Remove(vehicule);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id:int}/historique")]
    public async Task<ActionResult<IEnumerable<PositionGps>>> GetHistorique(
        int id,
        [FromQuery] DateTime? dateDebut,
        [FromQuery] DateTime? dateFin)
    {
        var vehiculeExiste = await _context.Vehicules.AnyAsync(v => v.Id == id);
        if (!vehiculeExiste)
            return NotFound(new { message = $"Véhicule {id} introuvable." });

        if (dateDebut.HasValue && dateFin.HasValue && dateDebut > dateFin)
            return BadRequest(new { message = "La date de début doit être antérieure à la date de fin." });

        var query = _context.PositionsGps
            .AsNoTracking()
            .Where(p => p.VehiculeId == id);

        if (dateDebut.HasValue)
            query = query.Where(p => p.DatePosition >= dateDebut.Value);

        if (dateFin.HasValue)
            query = query.Where(p => p.DatePosition <= dateFin.Value);

        var positions = await query
            .OrderBy(p => p.DatePosition)
            .ToListAsync();

        return Ok(positions);
    }

    [HttpGet("{id:int}/distance")]
    public async Task<ActionResult<DistanceResult>> GetDistance(int id)
    {
        var vehiculeExiste = await _context.Vehicules.AnyAsync(v => v.Id == id);
        if (!vehiculeExiste)
            return NotFound(new { message = $"Véhicule {id} introuvable." });

        var positions = await _context.PositionsGps
            .AsNoTracking()
            .Where(p => p.VehiculeId == id)
            .OrderBy(p => p.DatePosition)
            .ToListAsync();

        var distanceKm = _distanceService.CalculerDistanceTotale(positions);

        return Ok(new DistanceResult
        {
            VehiculeId = id,
            DistanceKm = distanceKm
        });
    }

    [HttpGet("{id:int}/zones")]
    public async Task<ActionResult<ZonesResult>> GetZones(int id)
    {
        var vehiculeExiste = await _context.Vehicules.AnyAsync(v => v.Id == id);
        if (!vehiculeExiste)
            return NotFound(new { message = $"Véhicule {id} introuvable." });

        var positions = await _context.PositionsGps
            .AsNoTracking()
            .Where(p => p.VehiculeId == id)
            .OrderBy(p => p.DatePosition)
            .ToListAsync();

        var zones = _zoneService.ObtenirZonesTraversees(positions);

        return Ok(new ZonesResult
        {
            VehiculeId = id,
            Zones = zones
        });
    }

    /// <summary>
    /// Retourne la dernière position GPS connue du véhicule.
    /// </summary>
    [HttpGet("{id:int}/position-actuelle")]
    public async Task<ActionResult<PositionActuelleResult>> GetPositionActuelle(int id)
    {
        var vehicule = await _context.Vehicules
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicule is null)
            return NotFound(new { message = $"Véhicule {id} introuvable." });

        var derniere = await _context.PositionsGps
            .AsNoTracking()
            .Where(p => p.VehiculeId == id)
            .OrderByDescending(p => p.DatePosition)
            .FirstOrDefaultAsync();

        if (derniere is null)
            return NotFound(new { message = $"Aucune position GPS enregistrée pour le véhicule {id}." });

        return Ok(new PositionActuelleResult
        {
            VehiculeId      = id,
            Immatriculation = vehicule.Immatriculation,
            Latitude        = derniere.Latitude,
            Longitude       = derniere.Longitude,
            DatePosition    = derniere.DatePosition
        });
    }

    /// <summary>
    /// Calcule la vitesse sur chaque segment entre positions successives.
    /// Retourne vitesse moyenne, maximale et le détail par segment.
    /// </summary>
    [HttpGet("{id:int}/vitesse")]
    public async Task<ActionResult<VitesseResult>> GetVitesse(int id)
    {
        var vehiculeExiste = await _context.Vehicules.AnyAsync(v => v.Id == id);
        if (!vehiculeExiste)
            return NotFound(new { message = $"Véhicule {id} introuvable." });

        var positions = await _context.PositionsGps
            .AsNoTracking()
            .Where(p => p.VehiculeId == id)
            .OrderBy(p => p.DatePosition)
            .ToListAsync();

        var resultat = _distanceService.CalculerVitesses(id, positions);

        if (resultat is null)
            return BadRequest(new { message = "Au moins 2 positions GPS sont nécessaires pour calculer une vitesse." });

        return Ok(resultat);
    }

    /// <summary>
    /// Retourne le statut de mouvement du véhicule (En mouvement / À l'arrêt / Inactif).
    /// </summary>
    [HttpGet("{id:int}/statut")]
    public async Task<ActionResult<StatutResult>> GetStatut(int id)
    {
        var vehiculeExiste = await _context.Vehicules.AnyAsync(v => v.Id == id);
        if (!vehiculeExiste)
            return NotFound(new { message = $"Véhicule {id} introuvable." });

        var positions = await _context.PositionsGps
            .AsNoTracking()
            .Where(p => p.VehiculeId == id)
            .OrderBy(p => p.DatePosition)
            .ToListAsync();

        var statut = _statutService.DeterminerStatut(id, positions);
        return Ok(statut);
    }
}
