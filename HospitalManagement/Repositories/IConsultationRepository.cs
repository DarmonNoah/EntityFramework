using HospitalManagement.Models;

namespace HospitalManagement.Repositories;

public interface IConsultationRepository : IRepository<Consultation>
{
    Task<IEnumerable<Consultation>> GetUpcomingForPatientAsync(int patientId);
    Task<IEnumerable<Consultation>> GetTodayForDoctorAsync(int doctorId);
    Task<bool> HasConflictAsync(int patientId, int doctorId, DateTime date);
}