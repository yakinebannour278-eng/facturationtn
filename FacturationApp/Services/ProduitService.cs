using Microsoft.EntityFrameworkCore;
using FacturationApp.Data;
using FacturationApp.Models;

namespace FacturationApp.Services;

public interface IProduitService
{
    Task<List<Produit>> GetAllAsync(string? search = null, decimal? tauxTVA = null, bool? actif = null);
    Task<Produit?> GetByIdAsync(int id);
    Task<Produit> CreateAsync(Produit produit);
    Task<Produit> UpdateAsync(Produit produit);
    Task DeleteAsync(int id);
    Task<List<decimal>> GetTauxTVADistinctsAsync();
}

public class ProduitService : IProduitService
{
    private readonly AppDbContext _db;

    public ProduitService(AppDbContext db) => _db = db;

    public async Task<List<Produit>> GetAllAsync(string? search = null, decimal? tauxTVA = null, bool? actif = null)
    {
        IQueryable<Produit> query = _db.Produits.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(p =>
                p.Reference.ToLower().Contains(search) ||
                p.Designation.ToLower().Contains(search) ||
                (p.Categorie != null && p.Categorie.ToLower().Contains(search)));
        }

        if (tauxTVA.HasValue)
            query = query.Where(p => p.TauxTVA == tauxTVA.Value);

        if (actif.HasValue)
            query = query.Where(p => p.Actif == actif.Value);

        return await query.OrderBy(p => p.Reference).ToListAsync();
    }

    public async Task<Produit?> GetByIdAsync(int id) =>
        await _db.Produits.FindAsync(id);

    public async Task<Produit> CreateAsync(Produit produit)
    {
        produit.DateCreation = DateTime.Now;
        _db.Produits.Add(produit);
        await _db.SaveChangesAsync();
        return produit;
    }

    public async Task<Produit> UpdateAsync(Produit produit)
    {
        _db.Produits.Update(produit);
        await _db.SaveChangesAsync();
        return produit;
    }

    public async Task DeleteAsync(int id)
    {
        var produit = await _db.Produits.FindAsync(id);
        if (produit != null)
        {
            produit.Actif = false;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<List<decimal>> GetTauxTVADistinctsAsync() =>
        await _db.Produits.Select(p => p.TauxTVA).Distinct().OrderBy(t => t).ToListAsync();
}
