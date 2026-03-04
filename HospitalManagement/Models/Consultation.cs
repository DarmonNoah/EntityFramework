using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HospitalManagement.Models;

public enum ConsultationStatus
{
    Planned,
    Completed,
    Cancelled
}

public class Consultation
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public ConsultationStatus Status { get; set; } = ConsultationStatus.Planned;

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Clés étrangères
    public int PatientId { get; set; }
    public int DoctorId { get; set; }

    // Navigation — ignorées lors de la désérialisation JSON
    [JsonIgnore]
    public Patient Patient { get; set; } = null!;

    [JsonIgnore]
    public Doctor Doctor { get; set; } = null!;
}