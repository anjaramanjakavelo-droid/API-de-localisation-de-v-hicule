using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VehicleTrackingApi.Models;

public class PositionGps
{
    public int Id { get; set; }

    [Required(ErrorMessage = "L'identifiant du véhicule est obligatoire.")]
    public int VehiculeId { get; set; }

    [Required(ErrorMessage = "La latitude est obligatoire.")]
    [Range(-90, 90, ErrorMessage = "La latitude doit être comprise entre -90 et 90.")]
    [Column(TypeName = "decimal(10,7)")]
    public decimal Latitude { get; set; }

    [Required(ErrorMessage = "La longitude est obligatoire.")]
    [Range(-180, 180, ErrorMessage = "La longitude doit être comprise entre -180 et 180.")]
    [Column(TypeName = "decimal(10,7)")]
    public decimal Longitude { get; set; }

    [Required(ErrorMessage = "La date de position est obligatoire.")]
    public DateTime DatePosition { get; set; }

    [JsonIgnore]
    public Vehicule? Vehicule { get; set; }
}
