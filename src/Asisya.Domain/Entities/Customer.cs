namespace Asisya.Domain.Entities;

public class Customer
{
  public int Id { get; set; }
  public string CompanyName { get; set; } = string.Empty;
  public string? ContactName { get; set; }
  public string? Phone { get; set; }
  public ICollection<Order> Orders { get; set; } = new List<Order>();
}
