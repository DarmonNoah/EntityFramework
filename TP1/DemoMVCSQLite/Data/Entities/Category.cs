namespace DemoMVCSQLite.Data.Entities;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // Navigation
    public ICollection<Product> Products { get; set; } = [];
}
