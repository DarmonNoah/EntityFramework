using Microsoft.EntityFrameworkCore;
using HospitalManagement.Models;

namespace HospitalManagement.Data;

public class HospitalDbContext : DbContext
{
    public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Consultation> Consultations => Set<Consultation>();

    // Héritage TPH
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<MedicalDoctor> MedicalDoctors => Set<MedicalDoctor>();
    public DbSet<Nurse> Nurses => Set<Nurse>();
    public DbSet<AdminStaff> AdminStaffs => Set<AdminStaff>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---- Patient ----
        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.FileNumber)
            .IsUnique();

        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.Email)
            .IsUnique();

        // Owned Type : Address intégré dans la table Patients
        modelBuilder.Entity<Patient>()
            .OwnsOne(p => p.Address, a =>
            {
                a.Property(x => x.Street).HasMaxLength(200);
                a.Property(x => x.City).HasMaxLength(100);
                a.Property(x => x.PostalCode).HasMaxLength(10);
                a.Property(x => x.Country).HasMaxLength(100);
            });

        // ---- Doctor <-> Department ----
        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.Department)
            .WithMany(dep => dep.Doctors)
            .HasForeignKey(d => d.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Doctor>()
            .HasIndex(d => d.LicenseNumber)
            .IsUnique();

        // ---- Department -> HeadDoctor ----
        modelBuilder.Entity<Department>()
            .HasOne(dep => dep.HeadDoctor)
            .WithMany()
            .HasForeignKey(dep => dep.HeadDoctorId)
            .OnDelete(DeleteBehavior.NoAction);

        // ---- Hiérarchie de départements ----
        modelBuilder.Entity<Department>()
            .HasOne(d => d.ParentDepartment)
            .WithMany(d => d.SubDepartments)
            .HasForeignKey(d => d.ParentDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---- Consultation ----
        modelBuilder.Entity<Consultation>()
            .HasOne(c => c.Patient)
            .WithMany(p => p.Consultations)
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Consultation>()
            .HasOne(c => c.Doctor)
            .WithMany(d => d.Consultations)
            .HasForeignKey(c => c.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Consultation>()
            .HasIndex(c => new { c.PatientId, c.DoctorId, c.Date })
            .IsUnique();

        // ---- Héritage TPH Staff ----
        // EF Core gère automatiquement le discriminateur
        modelBuilder.Entity<Staff>()
            .HasDiscriminator<string>("StaffType")
            .HasValue<MedicalDoctor>("Doctor")
            .HasValue<Nurse>("Nurse")
            .HasValue<AdminStaff>("Admin");

        // Consultations d'un médecin pour une date donnée
        modelBuilder.Entity<Consultation>()
            .HasIndex(c => new { c.DoctorId, c.Date })
            .HasDatabaseName("IX_Consultations_DoctorId_Date");

        // Consultations d'un patient
        modelBuilder.Entity<Consultation>()
            .HasIndex(c => new { c.PatientId, c.Date })
            .HasDatabaseName("IX_Consultations_PatientId_Date");

        // Recherche patient par nom
        modelBuilder.Entity<Patient>()
            .HasIndex(p => new { p.LastName, p.FirstName })
            .HasDatabaseName("IX_Patients_LastName_FirstName");

        // Concurrency Token
        modelBuilder.Entity<Patient>()
            .Property(p => p.RowVersion)
            .IsRowVersion();
    }
}