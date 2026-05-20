using Microsoft.EntityFrameworkCore;
using FacturationApp.Data;
using FacturationApp.Models;

namespace FacturationApp.Services;

public interface IParametreService
{
    Task<string?> GetAsync(string cle);
    Task<decimal> GetDecimalAsync(string cle, decimal defaut = 0);
    Task SetAsync(string cle, string valeur);
    Task<List<Parametre>> GetAllAsync();
    Task<decimal> GetMontantTimbreDefautAsync();
}

public class ParametreService : IParametreService
{
    private readonly AppDbContext _db;

    public ParametreService(AppDbContext db) => _db = db;

    public async Task<string?> GetAsync(string cle) =>
        (await _db.Parametres.FirstOrDefaultAsync(p => p.Cle == cle))?.Valeur;

    public async Task<decimal> GetDecimalAsync(string cle, decimal defaut = 0)
    {
        var val = await GetAsync(cle);
        return decimal.TryParse(val, out var result) ? result : defaut;
    }

    public async Task SetAsync(string cle, string valeur)
    {
        var param = await _db.Parametres.FirstOrDefaultAsync(p => p.Cle == cle);
        if (param == null)
        {
            _db.Parametres.Add(new Parametre { Cle = cle, Valeur = valeur });
        }
        else
        {
            param.Valeur = valeur;
            _db.Parametres.Update(param);
        }
        await _db.SaveChangesAsync();
    }

    public async Task<List<Parametre>> GetAllAsync() =>
        await _db.Parametres.OrderBy(p => p.Categorie).ThenBy(p => p.Cle).ToListAsync();

    public async Task<decimal> GetMontantTimbreDefautAsync() =>
        await GetDecimalAsync("TIMBRE_MONTANT_DEFAUT", 1m);
}
