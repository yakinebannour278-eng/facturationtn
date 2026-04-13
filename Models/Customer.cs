using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.Models;

public class Customer
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string TaxId { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    
    public string ContactInformation { get; set; } = string.Empty;
}