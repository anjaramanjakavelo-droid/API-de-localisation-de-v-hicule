using System.ComponentModel.DataAnnotations;

namespace VehicleTrackingApi.Models;

public class Vehicule
{
    public int Id { get; set; }

    [Required(ErrorMessage = "L'immatriculation est obligatoire.")]
    [MaxLength(20)]
    public string Immatriculation { get; set; } = string.Empty;

    public ICollection<PositionGps> PositionsGps { get; set; } = new List<PositionGps>();
}
