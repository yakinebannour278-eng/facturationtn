using InvoicingSalesAnalysis.Models;

namespace InvoicingSalesAnalysis.Services;

public interface IDatabaseSeeder
{
    Task InitializeAsync();
}

public interface ICustomerService
{
    Task<List<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int id);
    Task SaveAsync(Customer customer);
    Task DeleteAsync(int id);
}

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task SaveAsync(Product product);
    Task DeleteAsync(int id);
}

public interface ISettingsService
{
    Task<decimal> GetTaxStampAmountAsync();
    Task SaveTaxStampAmountAsync(decimal amount);
}

public interface IInvoiceService
{
    Task<List<Invoice>> GetAllAsync();
    Task<Invoice?> GetByIdAsync(int id);
    Task<Invoice> CreateAsync(InvoiceDraft draft);
}

public interface IDashboardService
{
    Task<DashboardSummary> GetSummaryAsync(DateTime from, DateTime to, string periodGrouping);
}
