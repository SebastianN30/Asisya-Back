namespace Asisya.Domain.Entities;

public class Supplier
{
  public int Id { get; set; }
  public string CompanyName { get; set; } = string.Empty;
  public string? ContactName { get; set; }
  public string? Phone { get; set; }
  public ICollection<Product> Products { get; set; } = new List<Product>();
}
