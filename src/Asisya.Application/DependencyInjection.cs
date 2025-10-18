using Microsoft.Extensions.DependencyInjection;
using Asisya.Application.Services;

namespace Asisya.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddScoped<ProductService>();
    services.AddScoped<CategoryService>();
    return services;
  }
}
