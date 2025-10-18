using Asisya.Domain.Entities;
using Asisya.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Asisya.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
  private readonly AppDbContext _db;
  public CategoryRepository(AppDbContext db) => _db = db;

  public Task<Category?> GetByIdAsync(int id) => _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
  public Task<Category?> GetByNameAsync(string name) => _db.Categories.FirstOrDefaultAsync(c => c.Name == name);
  public async Task<IEnumerable<Category>> ListAsync() => await _db.Categories.OrderBy(c => c.Id).ToListAsync();
  public async Task AddAsync(Category category)
  {
    _db.Categories.Add(category);
    await _db.SaveChangesAsync();
  }
}
