using Microsoft.EntityFrameworkCore;
using HospitalManagement.Data;
using HospitalManagement.Models;

namespace HospitalManagement.Repositories;

public class ConsultationRepository : IConsultationRepository
{
    private readonly HospitalDbContext _context;

    public ConsultationRepository(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<Consultation?> GetByIdAsync(int id)
        => await _context.Consultations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Consultation>> GetAllAsync()
        => await _context.Consultations
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Consultation>> GetUpcomingForPatientAsync(int patientId)
        => await _context.Consultations
            .AsNoTracking()
            .Include(c => c.Doctor)
            .Where(c => c.PatientId == patientId
                     && c.Date > DateTime.Now
                     && c.Status == ConsultationStatus.Planned)
            .OrderBy(c => c.Date)
            .ToListAsync();

    public async Task<IEnumerable<Consultation>> GetTodayForDoctorAsync(int doctorId)
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        return await _context.Consultations
            .AsNoTracking()
            .Include(c => c.Patient)
            .Where(c => c.DoctorId == doctorId
                     && c.Date >= today
                     && c.Date < tomorrow
                     && c.Status != ConsultationStatus.Cancelled)
            .OrderBy(c => c.Date)
            .ToListAsync();
    }

    // Vérifie si un créneau est déjà pris
    public async Task<bool> HasConflictAsync(int patientId, int doctorId, DateTime date)
        => await _context.Consultations
            .AnyAsync(c => c.PatientId == patientId
                        && c.DoctorId == doctorId
                        && c.Date == date
                        && c.Status != ConsultationStatus.Cancelled);

    public async Task<Consultation> AddAsync(Consultation consultation)
    {
        _context.Consultations.Add(consultation);
        await _context.SaveChangesAsync();
        return consultation;
    }

    public async Task<Consultation> UpdateAsync(Consultation consultation)
    {
        _context.Consultations.Update(consultation);
        await _context.SaveChangesAsync();
        return consultation;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var consultation = await _context.Consultations.FindAsync(id);
        if (consultation is null) return false;

        _context.Consultations.Remove(consultation);
        await _context.SaveChangesAsync();
        return true;
    }
}