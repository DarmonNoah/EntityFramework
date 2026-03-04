using Microsoft.EntityFrameworkCore;
using HospitalManagement.Data;
using HospitalManagement.Models;

namespace HospitalManagement.Services;

public class PatientService : IPatientService
{
    private readonly HospitalDbContext _context;

    public PatientService(HospitalDbContext context)
    {
        _context = context;
    }

    // Lister tous les patients avec pagination, triés alphabétiquement
    public async Task<IEnumerable<Patient>> GetAllAsync(int page, int pageSize)
    {
        return await _context.Patients
            .AsNoTracking()
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Patient?> GetByIdAsync(int id)
    {
        return await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Patient?> GetByFileNumberAsync(string fileNumber)
    {
        return await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.FileNumber == fileNumber);
    }

    // Recherche par nom ou prénom (insensible à la casse)
    public async Task<IEnumerable<Patient>> SearchByNameAsync(string name)
    {
        return await _context.Patients
            .AsNoTracking()
            .Where(p => p.LastName.Contains(name) || p.FirstName.Contains(name))
            .OrderBy(p => p.LastName)
            .ToListAsync();
    }

    public async Task<Patient> CreateAsync(Patient patient)
    {
        // Génération automatique du numéro de dossier
        patient.FileNumber = $"PAT-{DateTime.Now.Year}-{Guid.NewGuid().ToString()[..5].ToUpper()}";

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return patient;
    }

    public async Task<Patient?> UpdateAsync(int id, Patient updated)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient is null) return null;

        patient.LastName = updated.LastName;
        patient.FirstName = updated.FirstName;
        patient.DateOfBirth = updated.DateOfBirth;
        patient.Address = updated.Address;
        patient.Phone = updated.Phone;
        patient.Email = updated.Email;

        try
        {
            await _context.SaveChangesAsync();
            return patient;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Deux utilisateurs ont modifié le même patient simultanément
            var entry = ex.Entries.Single();
            var databaseValues = await entry.GetDatabaseValuesAsync();

            if (databaseValues is null)
                throw new Exception("Le patient a été supprimé par un autre utilisateur.");

            throw new Exception("Ce patient a été modifié par un autre utilisateur. Veuillez recharger et réessayer.");
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var patient = await _context.Patients
            .Include(p => p.Consultations)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient is null) return false;

        // Suppression en cascade des consultations liées
        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
        return true;
    }
}