using InvoicingSalesAnalysis.Data;
using InvoicingSalesAnalysis.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoicingSalesAnalysis.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISettingsService _settingsService;

    public InvoiceService(IDbContextFactory<AppDbContext> contextFactory, ISettingsService settingsService)
    {
        _contextFactory = contextFactory;
        _settingsService = settingsService;
    }

    public async Task<List<Invoice>> GetAllAsync()
    {
        await using var db = await _contextFactory.CreateDbContextAsync();
        return await db.Invoices
            .Include(x => x.Customer)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
            .OrderByDescending(x => x.IssueDate)
            .ThenByDescending(x => x.Id)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Invoice?> GetByIdAsync(int id)
    {
        await using var db = await _contextFactory.CreateDbContextAsync();
        return await db.Invoices
            .Include(x => x.Customer)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Invoice> CreateAsync(InvoiceDraft draft)
    {
        if (draft.Lines.Count == 0)
        {
            throw new InvalidOperationException("An invoice must contain at least one line.");
        }

        await using var db = await _contextFactory.CreateDbContextAsync();
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == draft.CustomerId)
            ?? throw new InvalidOperationException("Please choose a valid customer.");

        var products = await db.Products
            .Where(x => draft.Lines.Select(l => l.ProductId).Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        var taxStampAmount = await _settingsService.GetTaxStampAmountAsync();
        var nextNumber = await BuildNextInvoiceNumberAsync(db);

        var invoice = new Invoice
        {
            CustomerId = customer.Id,
            IssueDate = draft.IssueDate.Date,
            InvoiceNumber = nextNumber,
            TaxStampAmount = taxStampAmount
        };

        foreach (var lineDraft in draft.Lines)
        {
            if (!products.TryGetValue(lineDraft.ProductId, out var product))
            {
                throw new InvalidOperationException("One or more selected products no longer exist.");
            }

            var quantity = decimal.Round(lineDraft.Quantity, 3);
            var unitPrice = decimal.Round(product.UnitPricePreTax, 3);
            var vatRate = decimal.Round(product.VatRate, 3);
            var subtotalPreTax = decimal.Round(quantity * unitPrice, 3);
            var vatAmount = decimal.Round(subtotalPreTax * vatRate, 3);
            var subtotalPostTax = decimal.Round(subtotalPreTax + vatAmount, 3);

            invoice.Lines.Add(new InvoiceLine
            {
                ProductId = product.Id,
                ProductNameSnapshot = product.Name,
                Quantity = quantity,
                UnitPricePreTax = unitPrice,
                VatRate = vatRate,
                SubtotalPreTax = subtotalPreTax,
                VatAmount = vatAmount,
                SubtotalPostTax = subtotalPostTax
            });
        }

        invoice.TotalPreTax = decimal.Round(invoice.Lines.Sum(x => x.SubtotalPreTax), 3);
        invoice.TotalVAT = decimal.Round(invoice.Lines.Sum(x => x.VatAmount), 3);
        invoice.TotalPostTax = decimal.Round(invoice.TotalPreTax + invoice.TotalVAT + invoice.TaxStampAmount, 3);

        db.Invoices.Add(invoice);
        await db.SaveChangesAsync();
        return invoice;
    }

    private static async Task<string> BuildNextInvoiceNumberAsync(AppDbContext db)
    {
        var lastNumber = await db.Invoices
            .OrderByDescending(x => x.Id)
            .Select(x => x.InvoiceNumber)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(lastNumber))
        {
            return "INV-000001";
        }

        var digits = new string(lastNumber.Where(char.IsDigit).ToArray());
        if (int.TryParse(digits, out var parsed))
        {
            return $"INV-{(parsed + 1):000000}";
        }

        return $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}";
    }
}
