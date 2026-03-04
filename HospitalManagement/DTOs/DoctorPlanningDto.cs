namespace HospitalManagement.DTOs;

public class DoctorPlanningDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public List<ConsultationSummaryDto> UpcomingConsultations { get; set; } = new();
}