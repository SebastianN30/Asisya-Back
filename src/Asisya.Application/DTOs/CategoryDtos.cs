namespace Asisya.Application.DTOs;
public record CreateCategoryDto(string Name, string? PhotoUrl);
public record CategoryDto(int Id, string Name, string? PhotoUrl);
