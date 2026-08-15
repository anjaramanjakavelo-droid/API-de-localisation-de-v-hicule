using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleTrackingApi.Data;
using VehicleTrackingApi.Models;

namespace VehicleTrackingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PositionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PositionGps>>> GetAll()
    {
        var positions = await _context.PositionsGps
            .AsNoTracking()
            .OrderByDescending(p => p.DatePosition)
            .ToListAsync();

        return Ok(positions);
    }

    [HttpPost]
    public async Task<ActionResult<PositionGps>> Create([FromBody] PositionGps position)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var vehiculeExiste = await _context.Vehicules
            .AnyAsync(v => v.Id == position.VehiculeId);

        if (!vehiculeExiste)
            return BadRequest(new { message = $"Véhicule {position.VehiculeId} introuvable." });

        _context.PositionsGps.Add(position);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = position.Id }, position);
    }
}
