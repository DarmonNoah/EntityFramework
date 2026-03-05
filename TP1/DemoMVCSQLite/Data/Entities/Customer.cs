namespace DemoMVCSQLite.Data.Entities;

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Email { get; set; }

    // Navigation
    public ICollection<Order> Orders { get; set; } = [];
}
