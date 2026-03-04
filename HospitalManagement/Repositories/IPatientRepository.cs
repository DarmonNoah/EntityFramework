using HospitalManagement.Models;

namespace HospitalManagement.Repositories;

public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetByFileNumberAsync(string fileNumber);
    Task<Patient?> GetByEmailAsync(string email);
    Task<IEnumerable<Patient>> SearchByNameAsync(string name);
    Task<IEnumerable<Patient>> GetPagedAsync(int page, int pageSize);
}