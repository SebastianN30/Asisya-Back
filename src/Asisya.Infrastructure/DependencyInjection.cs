using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Asisya.Domain.Interfaces;
using Asisya.Infrastructure.Repositories;

namespace Asisya.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
  {
    var conn = config.GetConnectionString("DefaultConnection") ?? "Host=db;Port=5432;Database=asisya_db;Username=postgres;Password=123";
    services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(conn));
    services.AddScoped<IProductRepository, ProductRepository>();
    services.AddScoped<ICategoryRepository, CategoryRepository>();
    return services;
  }

  public static async Task ApplyMigrationsAndSeedAsync(this IServiceProvider sp)
  {
    using var scope = sp.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Crear schema si no hay migraciones (desarrollo)
    await db.Database.EnsureCreatedAsync();

    // Semilla automática: SERVIDORES y CLOUD
    if (!await db.Categories.AnyAsync())
    {
      db.Categories.Add(new Domain.Entities.Category { Name = "SERVIDORES", PhotoUrl = "https://picsum.photos/seed/server/300/200" });
      db.Categories.Add(new Domain.Entities.Category { Name = "CLOUD", PhotoUrl = "https://picsum.photos/seed/cloud/300/200" });
      await db.SaveChangesAsync();
    }
  }
}
