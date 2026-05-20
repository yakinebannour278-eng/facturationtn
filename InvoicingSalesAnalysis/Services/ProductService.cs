using InvoicingSalesAnalysis.Data;
using InvoicingSalesAnalysis.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoicingSalesAnalysis.Services;

public sealed class ProductService : IProductService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public ProductService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        await using var db = await _contextFactory.CreateDbContextAsync();
        return await db.Products.OrderByDescending(x => x.IsActive).ThenBy(x => x.Name).AsNoTracking().ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        await using var db = await _contextFactory.CreateDbContextAsync();
        return await db.Products.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task SaveAsync(Product product)
    {
        await using var db = await _contextFactory.CreateDbContextAsync();
        if (product.Id == 0)
        {
            db.Products.Add(product);
        }
        else
        {
            db.Products.Update(product);
        }

        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var db = await _contextFactory.CreateDbContextAsync();
        var inUse = await db.InvoiceLines.AnyAsync(x => x.ProductId == id);
        if (inUse)
        {
            throw new InvalidOperationException("This product is used on one or more invoices and cannot be deleted.");
        }

        var product = await db.Products.FindAsync(id);
        if (product is null)
        {
            return;
        }

        db.Products.Remove(product);
        await db.SaveChangesAsync();
    }
}
