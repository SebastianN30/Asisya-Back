using Asisya.Domain.Entities;

namespace Asisya.Domain.Interfaces;

public interface ICategoryRepository
{
  Task<Category?> GetByIdAsync(int id);
  Task<Category?> GetByNameAsync(string name);
  Task<IEnumerable<Category>> ListAsync();
  Task AddAsync(Category category);
}
