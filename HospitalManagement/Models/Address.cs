using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Models;

// Owned Type : pas de table propre, intégré dans la table parent
[Owned]
public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public override string ToString() => $"{Street}, {PostalCode} {City}, {Country}";
}