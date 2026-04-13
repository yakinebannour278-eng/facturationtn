using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public decimal UnitPricePreTax { get; set; }

    // E.g., 7, 13, 19
    public decimal VatRate { get; set; }

    public bool IsActive { get; set; } = true;
}