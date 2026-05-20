namespace InvoicingSalesAnalysis.Models;

public sealed record VatBreakdown(decimal VatRate, decimal TotalVat);

public sealed record PeriodRevenueItem(string Label, decimal PreTax, decimal PostTax);

public sealed record NamedRevenueItem(string Name, decimal PreTax, decimal PostTax);

public sealed record DashboardSummary(
    decimal TotalRevenuePreTax,
    decimal TotalRevenuePostTax,
    decimal TotalVatCollected,
    decimal TotalTaxStampCollected,
    IReadOnlyList<VatBreakdown> VatByRate,
    IReadOnlyList<PeriodRevenueItem> RevenueByPeriod,
    IReadOnlyList<NamedRevenueItem> TopCustomers,
    IReadOnlyList<NamedRevenueItem> TopProducts);
