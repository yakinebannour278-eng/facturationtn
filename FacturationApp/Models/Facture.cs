using System.ComponentModel.DataAnnotations;

namespace FacturationApp.Models;

public enum StatutFacture
{
    Brouillon,
    Emise,
    Payee,
    Annulee
}

public class Facture
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public string Numero { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le client est obligatoire.")]
    public int ClientId { get; set; }
    public Client? Client { get; set; }

    [Required]
    public DateTime DateFacture { get; set; } = DateTime.Today;

    public DateTime? DateEcheance { get; set; }
    public StatutFacture Statut { get; set; } = StatutFacture.Brouillon;

    [StringLength(500)]
    public string? Notes { get; set; }

    public decimal MontantTimbre { get; set; } = 1m;
    public decimal TotalHT { get; set; }
    public decimal TotalTVA { get; set; }
    public decimal TotalTTC { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.Now;

    // Navigation
    public List<LigneFacture> Lignes { get; set; } = new List<LigneFacture>();
}

public class LigneFacture
{
    public int Id { get; set; }

    [Required]
    public int FactureId { get; set; }
    public Facture? Facture { get; set; }

    [Required(ErrorMessage = "Le produit est obligatoire.")]
    public int ProduitId { get; set; }
    public Produit? Produit { get; set; }

    [Required]
    [StringLength(200)]
    public string Designation { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "La quantite doit etre superieure a 0.")]
    public int Quantite { get; set; } = 1;

    [Required]
    public decimal PrixUnitaireHT { get; set; }

    [Required]
    [Range(0, 100)]
    public decimal TauxTVA { get; set; }

    // Computed (not stored)
    public decimal MontantHT => Quantite * PrixUnitaireHT;
    public decimal MontantTVA => MontantHT * TauxTVA / 100;
    public decimal MontantTTC => MontantHT + MontantTVA;
}

public class TVAParTaux
{
    public decimal Taux { get; set; }
    public decimal Assiette { get; set; }
    public decimal Montant { get; set; }
}
