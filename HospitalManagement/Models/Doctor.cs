using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models;

public class Doctor
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Specialty { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    public int DepartmentId { get; set; }

    // Nullable : EF le remplit via DepartmentId, pas besoin de le fournir en JSON
    public Department? Department { get; set; }

    public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
}