using Asisya.Application.DTOs;
using Asisya.Domain.Entities;
using Asisya.Domain.Interfaces;

namespace Asisya.Application.Services;

public class CategoryService
{
  private readonly ICategoryRepository _repo;
  public CategoryService(ICategoryRepository repo) => _repo = repo;

  public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
  {
    var existing = await _repo.GetByNameAsync(dto.Name);
    if (existing is not null) throw new InvalidOperationException("Category already exists");
    var entity = new Category { Name = dto.Name, PhotoUrl = dto.PhotoUrl };
    await _repo.AddAsync(entity);
    return new CategoryDto(entity.Id, entity.Name, entity.PhotoUrl);
  }

  public async Task<IEnumerable<CategoryDto>> ListAsync()
  {
    var items = await _repo.ListAsync();
    return items.Select(c => new CategoryDto(c.Id, c.Name, c.PhotoUrl));
  }
}
