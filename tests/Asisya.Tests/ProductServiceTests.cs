using Asisya.Application.DTOs;
using Asisya.Application.Services;
using Asisya.Domain.Entities;
using Asisya.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Asisya.Tests;

public class ProductServiceTests
{
  [Fact]
  public async Task Create_Product_Returns_Dto_With_Category_Name()
  {
    var prodRepo = new Mock<IProductRepository>();
    var catRepo = new Mock<ICategoryRepository>();
    catRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Category { Id = 1, Name = "CLOUD" });
    prodRepo.Setup(x => x.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

    var service = new ProductService(prodRepo.Object, catRepo.Object);
    var dto = new CreateProductDto("Test", 10m, 5, 1);
    var result = await service.CreateAsync(dto);

    result.Name.Should().Be("Test");
    result.Category.Should().Be("CLOUD");
  }
}
