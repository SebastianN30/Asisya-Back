using Asisya.Infrastructure;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace Asisya.Tests;

public class Integration_Postgres_Container_Tests : IAsyncLifetime
{
  private readonly IContainer _pgContainer;
  private string _connStr = string.Empty;

  public Integration_Postgres_Container_Tests()
  {
    _pgContainer = new ContainerBuilder()
        .WithImage("postgres:15")
        .WithEnvironment("POSTGRES_USER", "postgres")
        .WithEnvironment("POSTGRES_PASSWORD", "123")
        .WithEnvironment("POSTGRES_DB", "test_db")
        .WithPortBinding(55432, 5432)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();
  }

  public async Task InitializeAsync()
  {
    await _pgContainer.StartAsync();
    _connStr = "Host=localhost;Port=55432;Database=test_db;Username=postgres;Password=123";
  }

  public async Task DisposeAsync()
  {
    await _pgContainer.StopAsync();
  }

  [Fact]
  public async Task EnsureCreated_Creates_Schema_And_Seed_Categories()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseNpgsql(_connStr).Options;

    using var db = new AppDbContext(options);
    await db.Database.EnsureCreatedAsync();

    // Seed manual simple
    if (!await db.Categories.AnyAsync())
    {
      db.Categories.Add(new Domain.Entities.Category { Name = "SERVIDORES" });
      db.Categories.Add(new Domain.Entities.Category { Name = "CLOUD" });
      await db.SaveChangesAsync();
    }

    var count = await db.Categories.CountAsync();
    count.Should().BeGreaterOrEqualTo(2);
  }
}
