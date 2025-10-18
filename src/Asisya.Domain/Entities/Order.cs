namespace Asisya.Domain.Entities;

public class Order
{
  public int Id { get; set; }
  public DateTime OrderDate { get; set; } = DateTime.UtcNow;
  public int CustomerId { get; set; }
  public Customer Customer { get; set; } = null!;
  public int EmployeeId { get; set; }
  public Employee Employee { get; set; } = null!;
  public int ShipperId { get; set; }
  public Shipper Shipper { get; set; } = null!;
  public ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
}
