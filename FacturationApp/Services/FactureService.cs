using Microsoft.EntityFrameworkCore;
using FacturationApp.Data;
using FacturationApp.Models;

namespace FacturationApp.Services;

public interface IFactureService
{
    Task<List<Facture>> GetAllAsync(string? search = null, int? clientId = null,
        StatutFacture? statut = null, DateTime? dateDebut = null, DateTime? dateFin = null);
    Task<Facture?> GetByIdAsync(int id);
    Task<Facture> CreateAsync(Facture facture);
    Task<Facture> UpdateAsync(Facture facture);
    Task DeleteAsync(int id);
    Task<string> GenererNumeroAsync();
    void RecalculerTotaux(Facture facture);
    Task<decimal> GetTimbreDefautAsync();
}

public class FactureService : IFactureService
{
    private readonly AppDbContext _db;
    private readonly IParametreService _params;

    public FactureService(AppDbContext db, IParametreService parametreService)
    {
        _db = db;
        _params = parametreService;
    }

    public async Task<List<Facture>> GetAllAsync(
        string? search = null, int? clientId = null,
        StatutFacture? statut = null, DateTime? dateDebut = null, DateTime? dateFin = null)
    {
        IQueryable<Facture> query = _db.Factures
            .Include(f => f.Client)
            .Include(f => f.Lignes)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string s = search.ToLower();
            query = query.Where(f =>
                f.Numero.ToLower().Contains(s) ||
                f.Client!.Nom.ToLower().Contains(s) ||
                f.Client.Prenom.ToLower().Contains(s));
        }

        if (clientId.HasValue)
            query = query.Where(f => f.ClientId == clientId.Value);

        if (statut.HasValue)
            query = query.Where(f => f.Statut == statut.Value);

        if (dateDebut.HasValue)
            query = query.Where(f => f.DateFacture >= dateDebut.Value);

        if (dateFin.HasValue)
            query = query.Where(f => f.DateFacture <= dateFin.Value);

        return await query
            .OrderByDescending(f => f.DateFacture)
            .ThenByDescending(f => f.Id)
            .ToListAsync();
    }

    public async Task<Facture?> GetByIdAsync(int id) =>
        await _db.Factures
            .Include(f => f.Client)
            .Include(f => f.Lignes)
                .ThenInclude(l => l.Produit)
            .FirstOrDefaultAsync(f => f.Id == id);

    public async Task<Facture> CreateAsync(Facture facture)
    {
        if (string.IsNullOrEmpty(facture.Numero))
            facture.Numero = await GenererNumeroAsync();

        RecalculerTotaux(facture);
        facture.DateCreation = DateTime.Now;
        _db.Factures.Add(facture);
        await _db.SaveChangesAsync();
        return facture;
    }

    public async Task<Facture> UpdateAsync(Facture facture)
    {
        // Remove old lignes and re-insert to handle deletions cleanly
        var existing = await _db.Factures
            .Include(f => f.Lignes)
            .FirstOrDefaultAsync(f => f.Id == facture.Id);

        if (existing != null)
        {
            // Remove all old lines
            _db.LignesFacture.RemoveRange(existing.Lignes);

            // Update scalar fields
            existing.ClientId = facture.ClientId;
            existing.DateFacture = facture.DateFacture;
            existing.DateEcheance = facture.DateEcheance;
            existing.Statut = facture.Statut;
            existing.Notes = facture.Notes;
            existing.MontantTimbre = facture.MontantTimbre;

            // Add new lines
            foreach (var l in facture.Lignes)
            {
                l.Id = 0; // ensure insert
                existing.Lignes.Add(l);
            }

            RecalculerTotaux(existing);
            await _db.SaveChangesAsync();
            return existing;
        }

        RecalculerTotaux(facture);
        _db.Factures.Update(facture);
        await _db.SaveChangesAsync();
        return facture;
    }

    public async Task DeleteAsync(int id)
    {
        var facture = await _db.Factures.FindAsync(id);
        if (facture != null)
        {
            facture.Statut = StatutFacture.Annulee;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<string> GenererNumeroAsync()
    {
        int annee = DateTime.Now.Year;
        int count = await _db.Factures.CountAsync(f => f.DateFacture.Year == annee);
        return $"FAC-{annee}-{(count + 1):D4}";
    }

    public void RecalculerTotaux(Facture facture)
    {
        facture.TotalHT  = facture.Lignes.Sum(l => l.Quantite * l.PrixUnitaireHT);
        facture.TotalTVA = facture.Lignes.Sum(l => l.Quantite * l.PrixUnitaireHT * l.TauxTVA / 100m);
        facture.TotalTTC = facture.TotalHT + facture.TotalTVA + facture.MontantTimbre;
    }

    public async Task<decimal> GetTimbreDefautAsync() =>
        await _params.GetMontantTimbreDefautAsync();
}
