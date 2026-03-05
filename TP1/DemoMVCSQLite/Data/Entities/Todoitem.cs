namespace DemoMVCSQLite.Data.Entities;

public class Todoitem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool MyProperty { get; set; }
}
