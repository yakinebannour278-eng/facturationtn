using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FacturationApp.Models;

namespace FacturationApp.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Produit> Produits { get; set; }
    public DbSet<Facture> Factures { get; set; }
    public DbSet<LigneFacture> LignesFacture { get; set; }
    public DbSet<Parametre> Parametres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasIndex(c => c.Email).IsUnique();
            entity.Property(c => c.Nom).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<Produit>(entity =>
        {
            entity.HasIndex(p => p.Reference).IsUnique();
        });

        modelBuilder.Entity<Facture>(entity =>
        {
            entity.HasIndex(f => f.Numero).IsUnique();
            entity.HasOne(f => f.Client)
                  .WithMany(c => c.Factures)
                  .HasForeignKey(f => f.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LigneFacture>(entity =>
        {
            entity.Ignore(l => l.MontantHT);
            entity.Ignore(l => l.MontantTVA);
            entity.Ignore(l => l.MontantTTC);
            entity.HasOne(l => l.Facture)
                  .WithMany(f => f.Lignes)
                  .HasForeignKey(l => l.FactureId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(l => l.Produit)
                  .WithMany(p => p.LignesFacture)
                  .HasForeignKey(l => l.ProduitId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Parametre>(entity =>
        {
            entity.HasIndex(p => p.Cle).IsUnique();
            entity.Property(p => p.Cle).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Valeur).IsRequired().HasMaxLength(500);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>().HasData(
            new Client { Id = 1, Nom = "Ben Ali", Prenom = "Ahmed", Email = "ahmed.benali@example.tn", Telephone = "+216 71 000 001", Adresse = "12 Rue de la République", Ville = "Tunis", CodePostal = "1000", MatriculeFiscale = "1234567/A/M/000", DateCreation = new DateTime(2024, 1, 15), Actif = true },
            new Client { Id = 2, Nom = "Chaabane", Prenom = "Sonia", Email = "sonia.chaabane@example.tn", Telephone = "+216 73 000 002", Adresse = "45 Avenue Habib Bourguiba", Ville = "Sfax", CodePostal = "3000", MatriculeFiscale = "7654321/B/P/000", DateCreation = new DateTime(2024, 2, 10), Actif = true },
            new Client { Id = 3, Nom = "Gharbi", Prenom = "Mohamed", Email = "m.gharbi@example.tn", Telephone = "+216 72 000 003", Adresse = "8 Rue Ibn Khaldoun", Ville = "Sousse", CodePostal = "4000", MatriculeFiscale = "9988776/C/M/000", DateCreation = new DateTime(2024, 3, 5), Actif = true },
            new Client { Id = 4, Nom = "Meddeb", Prenom = "Fatma", Email = "fatma.meddeb@example.tn", Telephone = "+216 74 000 004", Adresse = "22 Rue 14 Janvier", Ville = "Monastir", CodePostal = "5000", MatriculeFiscale = "1122334/D/P/000", DateCreation = new DateTime(2024, 3, 20), Actif = true }
        );

        modelBuilder.Entity<Produit>().HasData(
            new Produit { Id = 1, Reference = "INF-001", Designation = "Ordinateur Portable", Description = "Laptop 15 pouces, 16GB RAM, 512GB SSD", PrixUnitaireHT = 1500m, TauxTVA = 19m, Categorie = "Informatique", Actif = true, DateCreation = new DateTime(2024, 1, 1) },
            new Produit { Id = 2, Reference = "INF-002", Designation = "Ecran 27 pouces", Description = "Moniteur Full HD IPS", PrixUnitaireHT = 450m, TauxTVA = 19m, Categorie = "Informatique", Actif = true, DateCreation = new DateTime(2024, 1, 1) },
            new Produit { Id = 3, Reference = "LOG-001", Designation = "Licence Office 365", Description = "Abonnement annuel", PrixUnitaireHT = 200m, TauxTVA = 19m, Categorie = "Logiciel", Actif = true, DateCreation = new DateTime(2024, 1, 1) },
            new Produit { Id = 4, Reference = "SRV-001", Designation = "Maintenance Informatique", Description = "Contrat maintenance mensuel", PrixUnitaireHT = 300m, TauxTVA = 19m, Categorie = "Service", Actif = true, DateCreation = new DateTime(2024, 1, 1) },
            new Produit { Id = 5, Reference = "MED-001", Designation = "Medicament Generique", Description = "Produit pharmaceutique exonere", PrixUnitaireHT = 25m, TauxTVA = 0m, Categorie = "Medical", Actif = true, DateCreation = new DateTime(2024, 1, 1) },
            new Produit { Id = 6, Reference = "ALI-001", Designation = "Produit Alimentaire", Description = "Denree alimentaire", PrixUnitaireHT = 10m, TauxTVA = 7m, Categorie = "Alimentaire", Actif = true, DateCreation = new DateTime(2024, 1, 1) }
        );

        modelBuilder.Entity<Parametre>().HasData(
            new Parametre { Id = 1, Cle = "TIMBRE_MONTANT_DEFAUT", Valeur = "1.000", Description = "Montant du timbre fiscal par defaut (DT)", Categorie = "Fiscal" },
            new Parametre { Id = 2, Cle = "SOCIETE_NOM", Valeur = "Ma Societe SARL", Description = "Nom de la societe", Categorie = "Societe" },
            new Parametre { Id = 3, Cle = "SOCIETE_MF", Valeur = "0000000/A/M/000", Description = "Matricule fiscale de la societe", Categorie = "Societe" },
            new Parametre { Id = 4, Cle = "SOCIETE_ADRESSE", Valeur = "12 Rue de la Liberte, Tunis 1000, Tunisie", Description = "Adresse de la societe", Categorie = "Societe" },
            new Parametre { Id = 5, Cle = "SOCIETE_TEL", Valeur = "+216 71 000 000", Description = "Telephone de la societe", Categorie = "Societe" },
            new Parametre { Id = 6, Cle = "FACTURE_DELAI_PAIEMENT", Valeur = "30", Description = "Delai de paiement par defaut (jours)", Categorie = "Facturation" },
            new Parametre { Id = 7, Cle = "FACTURE_MENTIONS_LEGALES", Valeur = "Paiement a 30 jours. Tout retard de paiement entraine des penalites.", Description = "Mentions legales sur les factures", Categorie = "Facturation" }
        );
    }
}
