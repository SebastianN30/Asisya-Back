namespace Asisya.Domain.Entities;

public class Shipper
{
  public int Id { get; set; }
  public string CompanyName { get; set; } = string.Empty;
  public string? Phone { get; set; }
  public ICollection<Order> Orders { get; set; } = new List<Order>();
}
