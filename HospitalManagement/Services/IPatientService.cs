using HospitalManagement.Models;

namespace HospitalManagement.Services;

public interface IPatientService
{
    Task<IEnumerable<Patient>> GetAllAsync(int page, int pageSize);
    Task<Patient?> GetByIdAsync(int id);
    Task<Patient?> GetByFileNumberAsync(string fileNumber);
    Task<IEnumerable<Patient>> SearchByNameAsync(string name);
    Task<Patient> CreateAsync(Patient patient);
    Task<Patient?> UpdateAsync(int id, Patient patient);
    Task<bool> DeleteAsync(int id);
}