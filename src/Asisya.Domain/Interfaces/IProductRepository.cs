using Asisya.Domain.Entities;

namespace Asisya.Domain.Interfaces;

public interface IProductRepository
{
  Task<IEnumerable<Product>> SearchAsync(int page, int pageSize, string? search, int? categoryId, decimal? minPrice, decimal? maxPrice);
  Task<int> CountAsync(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice);
  Task<Product?> GetByIdAsync(int id);
  Task AddAsync(Product product);
  Task AddRangeAsync(IEnumerable<Product> products);
  Task UpdateAsync(Product product);
  Task DeleteAsync(int id);
}
