using Microsoft.EntityFrameworkCore;
using HospitalManagement.Data;
using HospitalManagement.Models;

namespace HospitalManagement.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly HospitalDbContext _context;

    public PatientRepository(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<Patient?> GetByIdAsync(int id)
        => await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Patient>> GetAllAsync()
        => await _context.Patients
            .AsNoTracking()
            .OrderBy(p => p.LastName)
            .ToListAsync();

    public async Task<IEnumerable<Patient>> GetPagedAsync(int page, int pageSize)
        => await _context.Patients
            .AsNoTracking()
            .OrderBy(p => p.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<Patient?> GetByFileNumberAsync(string fileNumber)
        => await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.FileNumber == fileNumber);

    public async Task<Patient?> GetByEmailAsync(string email)
        => await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Email == email);

    public async Task<IEnumerable<Patient>> SearchByNameAsync(string name)
        => await _context.Patients
            .AsNoTracking()
            .Where(p => p.LastName.Contains(name) || p.FirstName.Contains(name))
            .OrderBy(p => p.LastName)
            .ToListAsync();

    public async Task<Patient> AddAsync(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return patient;
    }

    public async Task<Patient> UpdateAsync(Patient patient)
    {
        _context.Patients.Update(patient);
        await _context.SaveChangesAsync();
        return patient;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient is null) return false;

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
        return true;
    }
}