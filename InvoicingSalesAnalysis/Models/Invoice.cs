using System.ComponentModel.DataAnnotations;

namespace InvoicingSalesAnalysis.Models;

public class Invoice
{
    public int Id { get; set; }

    [Required]
    [MaxLength(40)]
    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime IssueDate { get; set; } = DateTime.Today;

    public int CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public decimal TotalPreTax { get; set; }

    public decimal TotalVAT { get; set; }

    public decimal TaxStampAmount { get; set; }

    public decimal TotalPostTax { get; set; }

    public ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
}
