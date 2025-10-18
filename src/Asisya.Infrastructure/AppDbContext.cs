using Asisya.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Asisya.Infrastructure;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  public DbSet<Product> Products => Set<Product>();
  public DbSet<Category> Categories => Set<Category>();
  public DbSet<Supplier> Suppliers => Set<Supplier>();
  public DbSet<Customer> Customers => Set<Customer>();
  public DbSet<Employee> Employees => Set<Employee>();
  public DbSet<Shipper> Shippers => Set<Shipper>();
  public DbSet<Order> Orders => Set<Order>();
  public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Category>(e =>
    {
      e.ToTable("categories");
      e.HasKey(x => x.Id);
      e.Property(x => x.Name).IsRequired().HasMaxLength(100);
      e.Property(x => x.PhotoUrl).HasMaxLength(500);
    });

    modelBuilder.Entity<Supplier>(e =>
    {
      e.ToTable("suppliers");
      e.HasKey(x => x.Id);
      e.Property(x => x.CompanyName).IsRequired().HasMaxLength(150);
      e.Property(x => x.ContactName).HasMaxLength(100);
      e.Property(x => x.Phone).HasMaxLength(50);
    });

    modelBuilder.Entity<Product>(e =>
    {
      e.ToTable("products");
      e.HasKey(x => x.Id);
      e.Property(x => x.Name).IsRequired().HasMaxLength(150);
      e.Property(x => x.Price).HasPrecision(18, 2);
      e.Property(x => x.Stock);
      e.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
      e.HasOne(x => x.Category).WithMany(c => c.Products).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
      e.HasOne(x => x.Supplier).WithMany(s => s.Products).HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.SetNull);
      e.HasIndex(x => x.Name);
      e.HasIndex(x => new { x.CategoryId, x.Price });
    });

    modelBuilder.Entity<Customer>(e =>
    {
      e.ToTable("customers");
      e.HasKey(x => x.Id);
      e.Property(x => x.CompanyName).IsRequired().HasMaxLength(150);
      e.Property(x => x.ContactName).HasMaxLength(100);
      e.Property(x => x.Phone).HasMaxLength(50);
    });

    modelBuilder.Entity<Employee>(e =>
    {
      e.ToTable("employees");
      e.HasKey(x => x.Id);
      e.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
      e.Property(x => x.LastName).IsRequired().HasMaxLength(100);
      e.Property(x => x.Title).HasMaxLength(100);
    });

    modelBuilder.Entity<Shipper>(e =>
    {
      e.ToTable("shippers");
      e.HasKey(x => x.Id);
      e.Property(x => x.CompanyName).IsRequired().HasMaxLength(150);
      e.Property(x => x.Phone).HasMaxLength(50);
    });

    modelBuilder.Entity<Order>(e =>
    {
      e.ToTable("orders");
      e.HasKey(x => x.Id);
      e.Property(x => x.OrderDate);
      e.HasOne(x => x.Customer).WithMany(c => c.Orders).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
      e.HasOne(x => x.Employee).WithMany(emp => emp.Orders).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
      e.HasOne(x => x.Shipper).WithMany(s => s.Orders).HasForeignKey(x => x.ShipperId).OnDelete(DeleteBehavior.Restrict);
    });

    modelBuilder.Entity<OrderDetail>(e =>
    {
      e.ToTable("order_details");
      e.HasKey(x => new { x.OrderId, x.ProductId });
      e.Property(x => x.Quantity);
      e.Property(x => x.UnitPrice).HasPrecision(18, 2);
      e.HasOne(od => od.Order).WithMany(o => o.Details).HasForeignKey(od => od.OrderId).OnDelete(DeleteBehavior.Cascade);
      e.HasOne(od => od.Product).WithMany().HasForeignKey(od => od.ProductId).OnDelete(DeleteBehavior.Restrict);
    });
  }
}
