using HospitalManagement.Models;

namespace HospitalManagement.Services;

public interface IConsultationService
{
    Task<Consultation> ScheduleAsync(Consultation consultation);
    Task<Consultation?> UpdateStatusAsync(int id, ConsultationStatus status);
    Task<bool> CancelAsync(int id);
    Task<IEnumerable<Consultation>> GetUpcomingForPatientAsync(int patientId);
    Task<IEnumerable<Consultation>> GetTodayForDoctorAsync(int doctorId);
}