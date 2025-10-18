namespace Asisya.Domain.Entities;

public class Employee
{
  public int Id { get; set; }
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string? Title { get; set; }
  public ICollection<Order> Orders { get; set; } = new List<Order>();
}
