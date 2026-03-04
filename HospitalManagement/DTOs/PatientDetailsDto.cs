namespace HospitalManagement.DTOs;

public class PatientDetailsDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string FileNumber { get; set; } = string.Empty;
    public List<ConsultationSummaryDto> Consultations { get; set; } = new();
}

public class ConsultationSummaryDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}