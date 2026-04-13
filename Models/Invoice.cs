using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.Models;

public class Invoice
{
    public int Id { get; set; }
    
    public string InvoiceNumber { get; set; } = string.Empty;
    
    public DateTime IssueDate { get; set; } = DateTime.Now;
    
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    
    public List<InvoiceLine> Lines { get; set; } = new();

    // Snapshot at creation
    public decimal TaxStampAmount { get; set; }

    public decimal TotalPreTax => Lines.Sum(l => l.SubtotalPreTax);
    
    public decimal TotalVat => Lines.Sum(l => l.SubtotalVat);
    
    public decimal TotalPostTax => TotalPreTax + TotalVat + TaxStampAmount;
}