using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceApp.Models;

public class InvoiceLine
{
    public int Id { get; set; }

    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public decimal Quantity { get; set; }

    // Snapshot values
    public decimal UnitPricePreTax { get; set; }
    public decimal VatRate { get; set; }

    // Computations
    public decimal SubtotalPreTax => Quantity * UnitPricePreTax;
    public decimal SubtotalVat => SubtotalPreTax * (VatRate / 100m);
    public decimal SubtotalPostTax => SubtotalPreTax + SubtotalVat;
}