using System.Globalization;
using InvoicingSalesAnalysis.Data;
using InvoicingSalesAnalysis.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoicingSalesAnalysis.Services;

public sealed class SettingsService : ISettingsService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public SettingsService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<decimal> GetTaxStampAmountAsync()
    {
        await using var db = await _contextFactory.CreateDbContextAsync();
        var setting = await db.SystemSettings.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Key == SystemSettingKeys.TaxStampAmount);

        return setting is null || !decimal.TryParse(setting.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed)
            ? 1.000m
            : decimal.Round(parsed, 3);
    }

    public async Task SaveTaxStampAmountAsync(decimal amount)
    {
        amount = decimal.Round(amount, 3);

        await using var db = await _contextFactory.CreateDbContextAsync();
        var setting = await db.SystemSettings.FirstOrDefaultAsync(x => x.Key == SystemSettingKeys.TaxStampAmount);
        if (setting is null)
        {
            db.SystemSettings.Add(new SystemSetting
            {
                Key = SystemSettingKeys.TaxStampAmount,
                Value = amount.ToString("0.000", CultureInfo.InvariantCulture)
            });
        }
        else
        {
            setting.Value = amount.ToString("0.000", CultureInfo.InvariantCulture);
            db.SystemSettings.Update(setting);
        }

        await db.SaveChangesAsync();
    }
}
