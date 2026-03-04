using Microsoft.EntityFrameworkCore;
using HospitalManagement.Data;
using HospitalManagement.Models;

namespace HospitalManagement.Services;

public class ConsultationService : IConsultationService
{
    private readonly HospitalDbContext _context;

    public ConsultationService(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<Consultation> ScheduleAsync(Consultation consultation)
    {
        _context.Consultations.Add(consultation);
        await _context.SaveChangesAsync();
        return consultation;
    }

    public async Task<Consultation?> UpdateStatusAsync(int id, ConsultationStatus status)
    {
        var consultation = await _context.Consultations.FindAsync(id);
        if (consultation is null) return null;

        consultation.Status = status;
        await _context.SaveChangesAsync();
        return consultation;
    }

    public async Task<bool> CancelAsync(int id)
    {
        var consultation = await _context.Consultations.FindAsync(id);
        if (consultation is null) return false;

        consultation.Status = ConsultationStatus.Cancelled;
        await _context.SaveChangesAsync();
        return true;
    }

    // Consultations à venir pour un patient (triées par date)
    public async Task<IEnumerable<Consultation>> GetUpcomingForPatientAsync(int patientId)
    {
        return await _context.Consultations
            .AsNoTracking()
            .Include(c => c.Doctor)
            .Where(c => c.PatientId == patientId
                     && c.Date > DateTime.Now
                     && c.Status == ConsultationStatus.Planned)
            .OrderBy(c => c.Date)
            .ToListAsync();
    }

    // Consultations du jour pour un médecin
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
}