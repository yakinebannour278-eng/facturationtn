using InvoicingSalesAnalysis.Data;
using InvoicingSalesAnalysis.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoicingSalesAnalysis.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public DashboardService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<DashboardSummary> GetSummaryAsync(DateTime from, DateTime to, string periodGrouping)
    {
        await using var db = await _contextFactory.CreateDbContextAsync();

        var invoices = await db.Invoices
            .Include(x => x.Customer)
            .Include(x => x.Lines)
            .AsNoTracking()
            .Where(x => x.IssueDate >= from.Date && x.IssueDate <= to.Date)
            .ToListAsync();

        var totalRevenuePreTax = invoices.Sum(x => x.TotalPreTax);
        var totalRevenuePostTax = invoices.Sum(x => x.TotalPostTax);
        var totalVatCollected = invoices.Sum(x => x.TotalVAT);
        var totalTaxStampCollected = invoices.Sum(x => x.TaxStampAmount);

        var vatByRate = invoices
            .SelectMany(x => x.Lines)
            .GroupBy(x => x.VatRate)
            .Select(g => new VatBreakdown(g.Key, g.Sum(x => x.VatAmount)))
            .OrderByDescending(x => x.TotalVat)
            .ToList();

        var periodRevenue = BuildPeriodRevenue(invoices, periodGrouping);

        var topCustomers = invoices
            .GroupBy(x => x.Customer?.Name ?? "Unknown")
            .Select(g => new NamedRevenueItem(g.Key, g.Sum(x => x.TotalPreTax), g.Sum(x => x.TotalPostTax)))
            .OrderByDescending(x => x.PreTax)
            .Take(5)
            .ToList();

        var topProducts = invoices
            .SelectMany(x => x.Lines)
            .GroupBy(x => x.ProductNameSnapshot)
            .Select(g => new NamedRevenueItem(g.Key, g.Sum(x => x.SubtotalPreTax), g.Sum(x => x.SubtotalPostTax)))
            .OrderByDescending(x => x.PreTax)
            .Take(5)
            .ToList();

        return new DashboardSummary(
            totalRevenuePreTax,
            totalRevenuePostTax,
            totalVatCollected,
            totalTaxStampCollected,
            vatByRate,
            periodRevenue,
            topCustomers,
            topProducts);
    }

    private static List<PeriodRevenueItem> BuildPeriodRevenue(IEnumerable<Invoice> invoices, string periodGrouping)
    {
        return periodGrouping switch
        {
            "Yearly" => invoices
                .GroupBy(x => x.IssueDate.Year)
                .Select(g => new PeriodRevenueItem(g.Key.ToString(), g.Sum(x => x.TotalPreTax), g.Sum(x => x.TotalPostTax)))
                .OrderBy(x => x.Label)
                .ToList(),
            "Monthly" => invoices
                .GroupBy(x => new { x.IssueDate.Year, x.IssueDate.Month })
                .Select(g => new PeriodRevenueItem($"{g.Key.Year}-{g.Key.Month:00}", g.Sum(x => x.TotalPreTax), g.Sum(x => x.TotalPostTax)))
                .OrderBy(x => x.Label)
                .ToList(),
            _ => invoices
                .GroupBy(x => x.IssueDate.Date)
                .Select(g => new PeriodRevenueItem(g.Key.ToString("yyyy-MM-dd"), g.Sum(x => x.TotalPreTax), g.Sum(x => x.TotalPostTax)))
                .OrderBy(x => x.Label)
                .ToList()
        };
    }
}
