using Asisya.Application.DTOs;
using Asisya.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asisya.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
  private readonly CategoryService _service;
  public CategoryController(CategoryService service) => _service = service;

  [Authorize]
  [HttpPost]
  public async Task<IActionResult> Create(CreateCategoryDto dto) =>
      Ok(await _service.CreateAsync(dto));

  [HttpGet]
  public async Task<IActionResult> List() => Ok(await _service.ListAsync());
}
