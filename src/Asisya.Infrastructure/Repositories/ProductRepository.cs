using Asisya.Domain.Entities;
using Asisya.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Asisya.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
  private readonly AppDbContext _db;
  public ProductRepository(AppDbContext db) => _db = db;

  public async Task<IEnumerable<Product>> SearchAsync(int page, int pageSize, string? search, int? categoryId, decimal? minPrice, decimal? maxPrice)
  {
    var q = _db.Products.Include(p => p.Category).AsQueryable();
    if (!string.IsNullOrWhiteSpace(search)) q = q.Where(p => p.Name.Contains(search));
    if (categoryId.HasValue) q = q.Where(p => p.CategoryId == categoryId.Value);
    if (minPrice.HasValue) q = q.Where(p => p.Price >= minPrice.Value);
    if (maxPrice.HasValue) q = q.Where(p => p.Price <= maxPrice.Value);
    return await q.OrderBy(p => p.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
  }

  public async Task<int> CountAsync(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice)
  {
    var q = _db.Products.AsQueryable();
    if (!string.IsNullOrWhiteSpace(search)) q = q.Where(p => p.Name.Contains(search));
    if (categoryId.HasValue) q = q.Where(p => p.CategoryId == categoryId.Value);
    if (minPrice.HasValue) q = q.Where(p => p.Price >= minPrice.Value);
    if (maxPrice.HasValue) q = q.Where(p => p.Price <= maxPrice.Value);
    return await q.CountAsync();
  }

  public Task<Product?> GetByIdAsync(int id) =>
      _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

  public async Task AddAsync(Product product)
  {
    _db.Products.Add(product);
    await _db.SaveChangesAsync();
    await _db.Entry(product).Reference(p => p.Category).LoadAsync();
  }

  public async Task AddRangeAsync(IEnumerable<Product> products)
  {
    await _db.Products.AddRangeAsync(products);
    await _db.SaveChangesAsync();
  }

  public async Task UpdateAsync(Product product)
  {
    _db.Products.Update(product);
    await _db.SaveChangesAsync();
  }

  public async Task DeleteAsync(int id)
  {
    var p = await _db.Products.FindAsync(id);
    if (p is null) return;
    _db.Products.Remove(p);
    await _db.SaveChangesAsync();
  }
}
