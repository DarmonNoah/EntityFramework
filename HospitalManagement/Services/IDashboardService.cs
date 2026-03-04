using HospitalManagement.DTOs;

namespace HospitalManagement.Services;

public interface IDashboardService
{
    Task<PatientDetailsDto?> GetPatientDetailsAsync(int patientId);
    Task<DoctorPlanningDto?> GetDoctorPlanningAsync(int doctorId);
    Task<IEnumerable<DepartmentStatsDto>> GetDepartmentStatsAsync();
}