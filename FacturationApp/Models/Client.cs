using System.ComponentModel.DataAnnotations;

namespace FacturationApp.Models;

public class Client
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(100, ErrorMessage = "Le nom ne doit pas dépasser 100 caractères.")]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est obligatoire.")]
    [StringLength(100)]
    public string Prenom { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est obligatoire.")]
    [EmailAddress(ErrorMessage = "Adresse email invalide.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le téléphone est obligatoire.")]
    [Phone(ErrorMessage = "Numéro de téléphone invalide.")]
    public string Telephone { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'adresse est obligatoire.")]
    public string Adresse { get; set; } = string.Empty;

    [StringLength(20)]
    public string? CodePostal { get; set; }

    [StringLength(100)]
    public string? Ville { get; set; }

    // Matricule fiscale (Tunisia-specific)
    [StringLength(20)]
    public string? MatriculeFiscale { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.Now;
    public bool Actif { get; set; } = true;

    // Navigation
    public ICollection<Facture> Factures { get; set; } = new List<Facture>();
}

