using InvoiceApp.Models;

namespace InvoiceApp.Services;

public interface IInvoiceService
{
    Task<List<Invoice>> GetAllAsync();
    Task<Invoice?> GetByIdAsync(int id);
    Task CreateAsync(Invoice invoice);
}