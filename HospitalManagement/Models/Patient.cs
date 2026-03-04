using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models;

public class Patient
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    [MaxLength(20)]
    public string FileNumber { get; set; } = string.Empty;

    public Address Address { get; set; } = new();

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    // Concurrency Token : détecte les modifications simultanées
    [Timestamp]
    public byte[]? RowVersion { get; set; }

    public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
}