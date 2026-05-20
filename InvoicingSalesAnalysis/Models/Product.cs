using System.ComponentModel.DataAnnotations;

namespace InvoicingSalesAnalysis.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal UnitPricePreTax { get; set; }

    [Range(typeof(decimal), "0", "1")]
    public decimal VatRate { get; set; } = 0.19m;

    public bool IsActive { get; set; } = true;

    public ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
}
