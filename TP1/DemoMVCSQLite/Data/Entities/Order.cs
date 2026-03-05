namespace DemoMVCSQLite.Data.Entities;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }

    // Navigation
    public Customer Customer { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
