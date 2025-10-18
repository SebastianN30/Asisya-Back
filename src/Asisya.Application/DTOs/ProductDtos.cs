namespace Asisya.Application.DTOs;
public record CreateProductDto(string Name, decimal Price, int Stock, int CategoryId);
public record ProductDto(int Id, string Name, decimal Price, int Stock, string Category, string? CategoryPhotoUrl);
public record PagedResult<T>(IEnumerable<T> Items, int Page, int PageSize, int TotalCount);
