using Asisya.Application.DTOs;
using Asisya.Domain.Entities;
using Asisya.Domain.Interfaces;

namespace Asisya.Application.Services;

public class ProductService
{
  private readonly IProductRepository _repo;
  private readonly ICategoryRepository _catRepo;
  public ProductService(IProductRepository repo, ICategoryRepository catRepo)
  {
    _repo = repo;
    _catRepo = catRepo;
  }

  public async Task<PagedResult<ProductDto>> SearchAsync(int page, int pageSize, string? q, int? categoryId, decimal? minPrice, decimal? maxPrice)
  {
    var items = await _repo.SearchAsync(page, pageSize, q, categoryId, minPrice, maxPrice);
    var total = await _repo.CountAsync(q, categoryId, minPrice, maxPrice);
    var mapped = items.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock, p.Category.Name, p.Category.PhotoUrl));
    return new PagedResult<ProductDto>(mapped, page, pageSize, total);
  }

  public async Task<ProductDto?> GetAsync(int id)
  {
    var p = await _repo.GetByIdAsync(id);
    return p is null ? null : new ProductDto(p.Id, p.Name, p.Price, p.Stock, p.Category.Name, p.Category.PhotoUrl);
  }

  public async Task<ProductDto> CreateAsync(CreateProductDto dto)
  {
    var cat = await _catRepo.GetByIdAsync(dto.CategoryId) ?? throw new InvalidOperationException("Category not found");
    var entity = new Product { Name = dto.Name, Price = dto.Price, Stock = dto.Stock, CategoryId = dto.CategoryId };
    await _repo.AddAsync(entity);
    return new ProductDto(entity.Id, entity.Name, entity.Price, entity.Stock, cat.Name, cat.PhotoUrl);
  }

  public async Task BulkGenerateAsync(int count, int? categoryId = null)
  {
    var rand = new Random();
    var categories = new List<int>();
    if (categoryId.HasValue)
    {
      categories.Add(categoryId.Value);
    }
    else
    {
      var list = await _catRepo.ListAsync();
      categories = list.Select(c => c.Id).ToList();
      if (!categories.Any()) throw new InvalidOperationException("No categories available");
    }

    var batch = new List<Product>(capacity: Math.Min(count, 5000));
    int remaining = count;
    while (remaining > 0)
    {
      batch.Clear();
      int take = Math.Min(remaining, 5000);
      for (int i = 0; i < take; i++)
      {
        var catId = categories[rand.Next(categories.Count)];
        var price = Math.Round((decimal)(rand.NextDouble() * 990 + 10), 2);
        batch.Add(new Product
        {
          Name = $"Product-{Guid.NewGuid():N}"[0..16],
          Price = price,
          Stock = rand.Next(0, 1000),
          CategoryId = catId
        });
      }
      await _repo.AddRangeAsync(batch);
      remaining -= take;
    }
  }

  public async Task UpdateAsync(int id, CreateProductDto dto)
  {
    var entity = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Product not found");
    entity.Name = dto.Name;
    entity.Price = dto.Price;
    entity.Stock = dto.Stock;
    entity.CategoryId = dto.CategoryId;
    await _repo.UpdateAsync(entity);
  }

  public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
}
