using System.ComponentModel.DataAnnotations;

namespace FacturationApp.Models;

public class Produit
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La reference est obligatoire.")]
    [StringLength(50)]
    public string Reference { get; set; } = string.Empty;

    [Required(ErrorMessage = "La designation est obligatoire.")]
    [StringLength(200)]
    public string Designation { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Le prix unitaire est obligatoire.")]
    [Range(0, double.MaxValue, ErrorMessage = "Le prix doit etre positif.")]
    public decimal PrixUnitaireHT { get; set; }

    [Required(ErrorMessage = "Le taux de TVA est obligatoire.")]
    [Range(0, 100, ErrorMessage = "Le taux de TVA doit etre entre 0 et 100.")]
    public decimal TauxTVA { get; set; } = 19m;

    [StringLength(100)]
    public string? Categorie { get; set; }

    public bool Actif { get; set; } = true;
    public DateTime DateCreation { get; set; } = DateTime.Now;

    // Navigation
    public ICollection<LigneFacture> LignesFacture { get; set; } = new List<LigneFacture>();
}
