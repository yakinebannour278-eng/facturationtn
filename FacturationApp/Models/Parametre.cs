using System.ComponentModel.DataAnnotations;

namespace FacturationApp.Models;

/// <summary>
/// Application-wide configurable parameters (e.g., tax stamp amount, company info).
/// Stored in DB so admin can update without redeployment.
/// </summary>
public class Parametre
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Cle { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Valeur { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    // Category for grouping in UI
    [StringLength(50)]
    public string Categorie { get; set; } = "General";
}
