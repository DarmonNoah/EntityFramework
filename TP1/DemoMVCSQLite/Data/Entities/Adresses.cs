using System;

namespace DemoMVCSQLite.Data.Entities;

public class Adresses
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Adresse { get; set; }

    // Navigation
    //public ICollection<Order> Orders { get; set; } = [];
}
