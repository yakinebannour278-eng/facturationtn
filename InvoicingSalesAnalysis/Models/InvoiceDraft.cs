namespace InvoicingSalesAnalysis.Models;

public sealed class InvoiceDraft
{
    public int CustomerId { get; set; }

    public DateTime IssueDate { get; set; } = DateTime.Today;

    public List<InvoiceLineDraft> Lines { get; set; } = new();
}

public sealed class InvoiceLineDraft
{
    public int ProductId { get; set; }

    public decimal Quantity { get; set; } = 1m;

    public string ProductName { get; set; } = string.Empty;

    public decimal UnitPricePreTax { get; set; }

    public decimal VatRate { get; set; }

    public decimal LinePreTax => Quantity * UnitPricePreTax;

    public decimal LineVat => LinePreTax * VatRate;

    public decimal LinePostTax => LinePreTax + LineVat;
}
