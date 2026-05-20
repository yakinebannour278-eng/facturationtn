using InvoicingSalesAnalysis.Data;
using InvoicingSalesAnalysis.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoicingSalesAnalysis.Services;

public sealed class DatabaseSeeder : IDatabaseSeeder
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public DatabaseSeeder(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task InitializeAsync()
    {
        await using var db = await _contextFactory.CreateDbContextAsync();
        await db.Database.EnsureCreatedAsync();

        if (!await db.Customers.AnyAsync())
        {
            db.Customers.AddRange(
                new Customer
                {
                    Name = "Société Atlas",
                    TaxId = "TN123456A",
                    Address = "Tunis, Tunisia",
                    ContactInfo = "atlas@example.com"
                },
                new Customer
                {
                    Name = "Café Sidi Bou Said",
                    TaxId = "TN987654B",
                    Address = "Sidi Bou Said",
                    ContactInfo = "+216 71 000 000"
                },
                new Customer
                {
                    Name = "Horizon Tech SARL",
                    TaxId = "TN555000C",
                    Address = "Sfax, Tunisia",
                    ContactInfo = "contact@horizon-tech.tn"
                });
        }

        if (!await db.Products.AnyAsync())
        {
            db.Products.AddRange(
                new Product { Name = "Consulting Service", UnitPricePreTax = 250.000m, VatRate = 0.19m, IsActive = true },
                new Product { Name = "Support Package", UnitPricePreTax = 120.000m, VatRate = 0.13m, IsActive = true },
                new Product { Name = "Training Session", UnitPricePreTax = 80.000m, VatRate = 0.07m, IsActive = true },
                new Product { Name = "Software License", UnitPricePreTax = 480.000m, VatRate = 0.19m, IsActive = true });
        }

        if (!await db.SystemSettings.AnyAsync(x => x.Key == SystemSettingKeys.TaxStampAmount))
        {
            db.SystemSettings.Add(new SystemSetting
            {
                Key = SystemSettingKeys.TaxStampAmount,
                Value = "1.000"
            });
        }

        await db.SaveChangesAsync();
    }
}
