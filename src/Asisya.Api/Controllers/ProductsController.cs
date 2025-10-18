using Asisya.Application.DTOs;
using Asisya.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asisya.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
  private readonly ProductService _service;
  public ProductsController(ProductService service) => _service = service;

  [HttpGet]
  public async Task<IActionResult> Search([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
      [FromQuery] string? q = null, [FromQuery] int? categoryId = null,
      [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null)
      => Ok(await _service.SearchAsync(page, pageSize, q, categoryId, minPrice, maxPrice));

  [HttpGet("{id:int}")]
  public async Task<IActionResult> Get(int id)
  {
    var result = await _service.GetAsync(id);
    return result is null ? NotFound() : Ok(result);
  }

  [Authorize]
  [HttpPost]
  public async Task<IActionResult> Create(CreateProductDto dto) => Ok(await _service.CreateAsync(dto));

  [Authorize]
  [HttpPost("bulk")]
  public async Task<IActionResult> Bulk([FromQuery] int count = 100000, [FromQuery] int? categoryId = null)
  {
    await _service.BulkGenerateAsync(count, categoryId);
    return Ok(new { inserted = count });
  }

  [Authorize]
  [HttpPut("{id:int}")]
  public async Task<IActionResult> Update(int id, CreateProductDto dto)
  {
    await _service.UpdateAsync(id, dto);
    return NoContent();
  }

  [Authorize]
  [HttpDelete("{id:int}")]
  public async Task<IActionResult> Delete(int id)
  {
    await _service.DeleteAsync(id);
    return NoContent();
  }
}
