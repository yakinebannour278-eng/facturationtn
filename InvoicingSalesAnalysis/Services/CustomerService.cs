using InvoicingSalesAnalysis.Data;
using InvoicingSalesAnalysis.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoicingSalesAnalysis.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public CustomerService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        await using var db = await _contextFactory.CreateDbContextAsync();
        return await db.Customers.OrderBy(x => x.Name).AsNoTracking().ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        await using var db = await _contextFactory.CreateDbContextAsync();
        return await db.Customers.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task SaveAsync(Customer customer)
    {
        await using var db = await _contextFactory.CreateDbContextAsync();
        if (customer.Id == 0)
        {
            db.Customers.Add(customer);
        }
        else
        {
            db.Customers.Update(customer);
        }

        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var db = await _contextFactory.CreateDbContextAsync();
        var hasInvoices = await db.Invoices.AnyAsync(x => x.CustomerId == id);
        if (hasInvoices)
        {
            throw new InvalidOperationException("This customer is linked to one or more invoices and cannot be deleted.");
        }

        var customer = await db.Customers.FindAsync(id);
        if (customer is null)
        {
            return;
        }

        db.Customers.Remove(customer);
        await db.SaveChangesAsync();
    }
}
