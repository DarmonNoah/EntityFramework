namespace HospitalManagement.DTOs;

public class CreateConsultationDto
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime Date { get; set; }
    public int Status { get; set; } = 0;
    public string? Notes { get; set; }
}