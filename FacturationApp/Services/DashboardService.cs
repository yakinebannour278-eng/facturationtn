using Microsoft.EntityFrameworkCore;
using FacturationApp.Data;
using FacturationApp.Models;

namespace FacturationApp.Services;

// DTOs for analytics
public record ChiffreAffaireParPeriode(string Periode, decimal TotalHT, decimal TotalTTC);
public record ChiffreAffaireParClient(int ClientId, string NomClient, decimal TotalHT, decimal TotalTTC, int NbFactures);
public record ChiffreAffaireParProduit(string Designation, decimal TotalHT, decimal TotalTTC, int Quantite);
public record TVAParTauxDto(decimal Taux, decimal Assiette, decimal MontantTVA);
public record DashboardKpi(decimal TotalHTAnnee, decimal TotalTTCAnnee, decimal TotalTVAAnnee, decimal TotalTimbres, int NbFactures, int NbClients, int NbProduits);

public interface IDashboardService
{
    Task<DashboardKpi> GetKpisAsync(int? annee = null);
    Task<List<ChiffreAffaireParPeriode>> GetCAParMoisAsync(int annee);
    Task<List<ChiffreAffaireParClient>> GetCAParClientAsync(DateTime? dateDebut = null, DateTime? dateFin = null);
    Task<List<ChiffreAffaireParProduit>> GetCAParProduitAsync(DateTime? dateDebut = null, DateTime? dateFin = null);
    Task<List<TVAParTauxDto>> GetTVAParTauxAsync(DateTime? dateDebut = null, DateTime? dateFin = null);
    Task<decimal> GetTotalTimbresAsync(DateTime? dateDebut = null, DateTime? dateFin = null);
}

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db) => _db = db;

    public async Task<DashboardKpi> GetKpisAsync(int? annee = null)
    {
        var anneeVal = annee ?? DateTime.Now.Year;
        var factures = _db.Factures
            .Include(f => f.Lignes)
            .Where(f => f.Statut != StatutFacture.Annulee && f.DateFacture.Year == anneeVal);

        var totalHT = await factures.SumAsync(f => f.TotalHT);
        var totalTTC = await factures.SumAsync(f => f.TotalTTC);
        var totalTVA = await factures.SumAsync(f => f.TotalTVA);
        var totalTimbres = await factures.SumAsync(f => f.MontantTimbre);
        var nbFactures = await factures.CountAsync();
        var nbClients = await _db.Clients.CountAsync(c => c.Actif);
        var nbProduits = await _db.Produits.CountAsync(p => p.Actif);

        return new DashboardKpi(totalHT, totalTTC, totalTVA, totalTimbres, nbFactures, nbClients, nbProduits);
    }

    public async Task<List<ChiffreAffaireParPeriode>> GetCAParMoisAsync(int annee)
    {
        var data = await _db.Factures
            .Where(f => f.Statut != StatutFacture.Annulee && f.DateFacture.Year == annee)
            .GroupBy(f => f.DateFacture.Month)
            .Select(g => new
            {
                Mois = g.Key,
                TotalHT = g.Sum(f => f.TotalHT),
                TotalTTC = g.Sum(f => f.TotalTTC)
            })
            .OrderBy(x => x.Mois)
            .ToListAsync();

        var moisNoms = new[] { "", "Jan", "Fév", "Mar", "Avr", "Mai", "Jun", "Jul", "Aoû", "Sep", "Oct", "Nov", "Déc" };
        return data.Select(d => new ChiffreAffaireParPeriode(moisNoms[d.Mois], d.TotalHT, d.TotalTTC)).ToList();
    }

    public async Task<List<ChiffreAffaireParClient>> GetCAParClientAsync(DateTime? dateDebut = null, DateTime? dateFin = null)
    {
        IQueryable<Facture> query = _db.Factures
            .Where(f => f.Statut != StatutFacture.Annulee);

        if (dateDebut.HasValue) query = query.Where(f => f.DateFacture >= dateDebut.Value);
        if (dateFin.HasValue) query = query.Where(f => f.DateFacture <= dateFin.Value);

        var data = await query
            .GroupBy(f => new { f.ClientId, f.Client!.Nom, f.Client.Prenom })
            .Select(g => new
            {
                g.Key.ClientId,
                g.Key.Nom,
                g.Key.Prenom,
                TotalHT = g.Sum(f => f.TotalHT),
                TotalTTC = g.Sum(f => f.TotalTTC),
                NbFactures = g.Count()
            })
            .OrderByDescending(x => x.TotalTTC)
            .ToListAsync();

        return data
            .Select(d => new ChiffreAffaireParClient(
                d.ClientId,
                d.Nom + " " + d.Prenom,
                d.TotalHT,
                d.TotalTTC,
                d.NbFactures))
            .ToList();
    }

    public async Task<List<ChiffreAffaireParProduit>> GetCAParProduitAsync(DateTime? dateDebut = null, DateTime? dateFin = null)
    {
        IQueryable<LigneFacture> query = _db.LignesFacture
            .Include(l => l.Facture)
            .Where(l => l.Facture!.Statut != StatutFacture.Annulee);

        if (dateDebut.HasValue) query = query.Where(l => l.Facture!.DateFacture >= dateDebut.Value);
        if (dateFin.HasValue) query = query.Where(l => l.Facture!.DateFacture <= dateFin.Value);

        var data = await query
            .GroupBy(l => l.Designation)
            .Select(g => new
            {
                Designation = g.Key,
                TotalHT = g.Sum(l => l.Quantite * l.PrixUnitaireHT),
                TotalTTC = g.Sum(l => l.Quantite * l.PrixUnitaireHT * (1 + l.TauxTVA / 100)),
                Quantite = g.Sum(l => l.Quantite)
            })
            .OrderByDescending(x => x.TotalHT)
            .Take(10)
            .ToListAsync();

        return data
            .Select(d => new ChiffreAffaireParProduit(
                d.Designation,
                d.TotalHT,
                d.TotalTTC,
                d.Quantite))
            .ToList();
    }

    public async Task<List<TVAParTauxDto>> GetTVAParTauxAsync(DateTime? dateDebut = null, DateTime? dateFin = null)
    {
        IQueryable<LigneFacture> query = _db.LignesFacture
            .Include(l => l.Facture)
            .Where(l => l.Facture!.Statut != StatutFacture.Annulee);

        if (dateDebut.HasValue) query = query.Where(l => l.Facture!.DateFacture >= dateDebut.Value);
        if (dateFin.HasValue) query = query.Where(l => l.Facture!.DateFacture <= dateFin.Value);

        var data = await query
            .GroupBy(l => l.TauxTVA)
            .Select(g => new
            {
                Taux = g.Key,
                Assiette = g.Sum(l => l.Quantite * l.PrixUnitaireHT),
                MontantTVA = g.Sum(l => l.Quantite * l.PrixUnitaireHT * g.Key / 100)
            })
            .OrderBy(x => x.Taux)
            .ToListAsync();

        return data
            .Select(d => new TVAParTauxDto(d.Taux, d.Assiette, d.MontantTVA))
            .ToList();
    }

    public async Task<decimal> GetTotalTimbresAsync(DateTime? dateDebut = null, DateTime? dateFin = null)
    {
        IQueryable<Facture> query = _db.Factures.Where(f => f.Statut != StatutFacture.Annulee);
        if (dateDebut.HasValue) query = query.Where(f => f.DateFacture >= dateDebut.Value);
        if (dateFin.HasValue) query = query.Where(f => f.DateFacture <= dateFin.Value);
        return await query.SumAsync(f => f.MontantTimbre);
    }
}
