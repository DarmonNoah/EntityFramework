using Microsoft.EntityFrameworkCore;
using HospitalManagement.Data;
using HospitalManagement.Models;
using HospitalManagement.Repositories;

namespace HospitalManagement.Tests;

public class ConsultationServiceTests
{
    private HospitalDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<HospitalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new HospitalDbContext(options);
    }

    private async Task<(Patient patient, Doctor doctor)> SeedDataAsync(HospitalDbContext context)
    {
        var department = new Department
        {
            Name = "Cardiologie",
            Location = "Bâtiment A"
        };
        context.Departments.Add(department);
        await context.SaveChangesAsync();

        var doctor = new Doctor
        {
            FirstName = "Paul",
            LastName = "Martin",
            Specialty = "Cardiologue",
            LicenseNumber = "LIC-001",
            DepartmentId = department.Id
        };
        context.Doctors.Add(doctor);

        var patient = new Patient
        {
            FirstName = "Jean",
            LastName = "Dupont",
            Email = "jean@test.com",
            Phone = "0600000000",
            FileNumber = "PAT-001",
            DateOfBirth = new DateTime(1985, 1, 1),
            Address = new Address()
        };
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        return (patient, doctor);
    }

    [Fact]
    public async Task ScheduleConsultation_ShouldPersistCorrectly()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new ConsultationRepository(context);
        var (patient, doctor) = await SeedDataAsync(context);

        var consultation = new Consultation
        {
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            Date = DateTime.Now.AddDays(1),
            Status = ConsultationStatus.Planned
        };

        // Act
        var created = await repo.AddAsync(consultation);

        // Assert
        Assert.NotNull(created);
        Assert.Equal(ConsultationStatus.Planned, created.Status);
        Assert.Equal(1, await context.Consultations.CountAsync());
    }

    [Fact]
    public async Task HasConflict_ShouldReturnTrue_WhenSlotTaken()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new ConsultationRepository(context);
        var (patient, doctor) = await SeedDataAsync(context);

        var date = DateTime.Now.AddDays(1);

        await repo.AddAsync(new Consultation
        {
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            Date = date,
            Status = ConsultationStatus.Planned
        });

        // Act
        var hasConflict = await repo.HasConflictAsync(patient.Id, doctor.Id, date);

        // Assert
        Assert.True(hasConflict);
    }

    [Fact]
    public async Task CancelConsultation_ShouldUpdateStatus()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new ConsultationRepository(context);
        var (patient, doctor) = await SeedDataAsync(context);

        var consultation = await repo.AddAsync(new Consultation
        {
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            Date = DateTime.Now.AddDays(1),
            Status = ConsultationStatus.Planned
        });

        // Act
        consultation.Status = ConsultationStatus.Cancelled;
        await repo.UpdateAsync(consultation);

        // Assert
        var updated = await context.Consultations.FindAsync(consultation.Id);
        Assert.Equal(ConsultationStatus.Cancelled, updated!.Status);
    }
}