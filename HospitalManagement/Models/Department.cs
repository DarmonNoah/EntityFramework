using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models;

public class Department
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    // Responsable médical
    public int? HeadDoctorId { get; set; }
    public Doctor? HeadDoctor { get; set; }

    // Hiérarchie : sous-départements
    public int? ParentDepartmentId { get; set; }
    public Department? ParentDepartment { get; set; }
    public ICollection<Department> SubDepartments { get; set; } = new List<Department>();

    // Médecins du département
    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}