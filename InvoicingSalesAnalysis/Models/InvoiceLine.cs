using System.ComponentModel.DataAnnotations;

namespace InvoicingSalesAnalysis.Models;

public class InvoiceLine
{
    public int Id { get; set; }

    public int InvoiceId { get; set; }

    public Invoice? Invoice { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProductNameSnapshot { get; set; } = string.Empty;

    public decimal Quantity { get; set; } = 1m;

    public decimal UnitPricePreTax { get; set; }

    public decimal VatRate { get; set; }

    public decimal SubtotalPreTax { get; set; }

    public decimal VatAmount { get; set; }

    public decimal SubtotalPostTax { get; set; }
}
