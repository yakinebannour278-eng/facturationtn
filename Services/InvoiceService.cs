using InvoiceApp.Data;
using InvoiceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApp.Services;

public class InvoiceService : IInvoiceService
{
    private readonly AppDbContext _context;

    public InvoiceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Invoice>> GetAllAsync()
    {
        return await _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Lines)
            .ThenInclude(l => l.Product)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync();
    }

    public async Task<Invoice?> GetByIdAsync(int id)
    {
        return await _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Lines)
            .ThenInclude(l => l.Product)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task CreateAsync(Invoice invoice)
    {
        // Snapshot the stamp amount
        var stampSetting = await _context.SystemSettings.FindAsync("TaxStampAmount");
        if (stampSetting != null && decimal.TryParse(stampSetting.Value, out decimal stampValue))
        {
            invoice.TaxStampAmount = stampValue;
        }
        else
        {
            invoice.TaxStampAmount = 1.000m; // Fallback
        }

        // Detach product references so EF doesn't try to add them as new
        foreach (var line in invoice.Lines)
        {
            line.Product = null; 
        }

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
    }
}