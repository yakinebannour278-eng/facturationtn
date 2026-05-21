using Microsoft.EntityFrameworkCore;
using FacturationApp.Data;
using FacturationApp.Models;

namespace FacturationApp.Services;
public interface IClientService
{
    Task<List<Client>> GetAllAsync(string? search = null, bool? actif = null);
    Task<Client?> GetByIdAsync(int id);
    Task<Client> CreateAsync(Client client);
    Task<Client> UpdateAsync(Client client);
    Task DeleteAsync(int id);
    Task<bool> HasFacturesAsync(int clientId);
}

public class ClientService : IClientService
{
    private readonly AppDbContext _db;

    public ClientService(AppDbContext db) => _db = db;

    public async Task<List<Client>> GetAllAsync(string? search = null, bool? actif = null)
    {
        IQueryable<Client> query = _db.Clients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(c =>
                c.Nom.ToLower().Contains(search) ||
                c.Prenom.ToLower().Contains(search) ||
                c.Email.ToLower().Contains(search) ||
                (c.Ville != null && c.Ville.ToLower().Contains(search)) ||
                (c.MatriculeFiscale != null && c.MatriculeFiscale.ToLower().Contains(search)));
        }

        if (actif.HasValue)
            query = query.Where(c => c.Actif == actif.Value);

        return await query.OrderBy(c => c.Nom).ThenBy(c => c.Prenom).ToListAsync();
    }

    public async Task<Client?> GetByIdAsync(int id) =>
        await _db.Clients.Include(c => c.Factures).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Client> CreateAsync(Client client)
    {
        client.DateCreation = DateTime.Now;
        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
        return client;
    }

    public async Task<Client> UpdateAsync(Client client)
    {
        _db.Clients.Update(client);
        await _db.SaveChangesAsync();
        return client;
    }

    public async Task DeleteAsync(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client != null)
        {
            client.Actif = false; // Soft delete
            await _db.SaveChangesAsync();
        }
    }

    public async Task<bool> HasFacturesAsync(int clientId) =>
        await _db.Factures.AnyAsync(f => f.ClientId == clientId);
}
