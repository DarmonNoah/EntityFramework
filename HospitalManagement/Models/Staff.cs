using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models;

// Classe de base abstraite
public abstract class Staff
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    public DateTime HireDate { get; set; }

    public decimal Salary { get; set; }
}

// Médecin
public class MedicalDoctor : Staff
{
    [MaxLength(100)]
    public string Specialty { get; set; } = string.Empty;

    [MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;
}

// Infirmier
public class Nurse : Staff
{
    [MaxLength(100)]
    public string Service { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Grade { get; set; } = string.Empty;
}

// Personnel administratif
public class AdminStaff : Staff
{
    [MaxLength(100)]
    public string Function { get; set; } = string.Empty;
}