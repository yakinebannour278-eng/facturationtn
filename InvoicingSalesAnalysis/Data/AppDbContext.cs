using InvoicingSalesAnalysis.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoicingSalesAnalysis.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Invoice> Invoices => Set<Invoice>();

    public DbSet<InvoiceLine> InvoiceLines => Set<InvoiceLine>();

    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.TaxId).HasMaxLength(32);
            entity.Property(x => x.Address).HasMaxLength(300);
            entity.Property(x => x.ContactInfo).HasMaxLength(200);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UnitPricePreTax).HasPrecision(18, 3);
            entity.Property(x => x.VatRate).HasPrecision(5, 3);
            entity.Property(x => x.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.Property(x => x.InvoiceNumber).HasMaxLength(40).IsRequired();
            entity.Property(x => x.TotalPreTax).HasPrecision(18, 3);
            entity.Property(x => x.TotalVAT).HasPrecision(18, 3);
            entity.Property(x => x.TaxStampAmount).HasPrecision(18, 3);
            entity.Property(x => x.TotalPostTax).HasPrecision(18, 3);
            entity.HasOne(x => x.Customer)
                .WithMany(x => x.Invoices)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InvoiceLine>(entity =>
        {
            entity.Property(x => x.ProductNameSnapshot).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Quantity).HasPrecision(18, 3);
            entity.Property(x => x.UnitPricePreTax).HasPrecision(18, 3);
            entity.Property(x => x.VatRate).HasPrecision(5, 3);
            entity.Property(x => x.SubtotalPreTax).HasPrecision(18, 3);
            entity.Property(x => x.VatAmount).HasPrecision(18, 3);
            entity.Property(x => x.SubtotalPostTax).HasPrecision(18, 3);
            entity.HasOne(x => x.Invoice)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Product)
                .WithMany(x => x.InvoiceLines)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(x => x.Key);
            entity.Property(x => x.Key).HasMaxLength(128);
            entity.Property(x => x.Value).HasMaxLength(128).IsRequired();
        });

        modelBuilder.Entity<SystemSetting>().HasData(new SystemSetting
        {
            Key = SystemSettingKeys.TaxStampAmount,
            Value = "1.000"
        });
    }
}
