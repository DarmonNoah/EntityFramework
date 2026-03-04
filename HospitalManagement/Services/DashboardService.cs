using Microsoft.EntityFrameworkCore;
using HospitalManagement.Data;
using HospitalManagement.DTOs;

namespace HospitalManagement.Services;

public class DashboardService : IDashboardService
{
    private readonly HospitalDbContext _context;

    public DashboardService(HospitalDbContext context)
    {
        _context = context;
    }

    // Fiche patient : Eager Loading avec Include
    // On charge tout d'un coup pour éviter le problème N+1
    public async Task<PatientDetailsDto?> GetPatientDetailsAsync(int patientId)
    {
        return await _context.Patients
            .AsNoTracking()
            .Where(p => p.Id == patientId)
            .Select(p => new PatientDetailsDto
            {
                Id = p.Id,
                FullName = $"{p.FirstName} {p.LastName}",
                FileNumber = p.FileNumber,
                // Projection directe : on ne charge que ce dont on a besoin
                Consultations = p.Consultations
                    .OrderByDescending(c => c.Date)
                    .Select(c => new ConsultationSummaryDto
                    {
                        Id = c.Id,
                        Date = c.Date,
                        Status = c.Status.ToString(),
                        DoctorName = $"{c.Doctor.FirstName} {c.Doctor.LastName}",
                        Notes = c.Notes
                    }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    // Planning médecin : Eager Loading avec Include chaîné
    public async Task<DoctorPlanningDto?> GetDoctorPlanningAsync(int doctorId)
    {
        return await _context.Doctors
            .AsNoTracking()
            .Where(d => d.Id == doctorId)
            .Select(d => new DoctorPlanningDto
            {
                Id = d.Id,
                FullName = $"{d.FirstName} {d.LastName}",
                Specialty = d.Specialty,
                DepartmentName = d.Department.Name,
                // Uniquement les consultations à venir
                UpcomingConsultations = d.Consultations
                    .Where(c => c.Date > DateTime.Now
                             && c.Status == HospitalManagement.Models.ConsultationStatus.Planned)
                    .OrderBy(c => c.Date)
                    .Select(c => new ConsultationSummaryDto
                    {
                        Id = c.Id,
                        Date = c.Date,
                        Status = c.Status.ToString(),
                        DoctorName = $"{c.Doctor.FirstName} {c.Doctor.LastName}",
                        Notes = c.Notes
                    }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    // Statistiques département : projection agrégée
    // Évite de charger toutes les entités en mémoire
    public async Task<IEnumerable<DepartmentStatsDto>> GetDepartmentStatsAsync()
    {
        return await _context.Departments
            .AsNoTracking()
            .Select(dep => new DepartmentStatsDto
            {
                Id = dep.Id,
                Name = dep.Name,
                Location = dep.Location,
                DoctorCount = dep.Doctors.Count(),
                // On compte les consultations via les médecins du département
                ConsultationCount = dep.Doctors
                    .SelectMany(d => d.Consultations)
                    .Count()
            })
            .ToListAsync();
    }
}