using HospitalManagement.Models;

namespace HospitalManagement.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(HospitalDbContext context)
    {
        // Ne seed que si la base est vide
        if (context.Departments.Any()) return;

        // ---- Départements ----
        var cardio = new Department { Name = "Cardiologie", Location = "Bâtiment A - 2ème étage" };
        var neuro = new Department { Name = "Neurologie", Location = "Bâtiment B - 1er étage" };
        var pediatrie = new Department { Name = "Pédiatrie", Location = "Bâtiment C - RDC" };

        context.Departments.AddRange(cardio, neuro, pediatrie);
        await context.SaveChangesAsync();

        // ---- Médecins ----
        var doctor1 = new Doctor
        {
            FirstName = "Paul",
            LastName = "Martin",
            Specialty = "Cardiologue",
            LicenseNumber = "LIC-2024-001",
            DepartmentId = cardio.Id
        };
        var doctor2 = new Doctor
        {
            FirstName = "Sophie",
            LastName = "Bernard",
            Specialty = "Neurologue",
            LicenseNumber = "LIC-2024-002",
            DepartmentId = neuro.Id
        };
        var doctor3 = new Doctor
        {
            FirstName = "Lucas",
            LastName = "Petit",
            Specialty = "Pédiatre",
            LicenseNumber = "LIC-2024-003",
            DepartmentId = pediatrie.Id
        };

        context.Doctors.AddRange(doctor1, doctor2, doctor3);
        await context.SaveChangesAsync();

        // ---- Patients ----
        var patient1 = new Patient
        {
            FirstName = "Jean",
            LastName = "Dupont",
            DateOfBirth = new DateTime(1985, 6, 15),
            FileNumber = "PAT-2024-001",
            Phone = "0600000001",
            Email = "jean.dupont@email.com",
            Address = new Address
            {
                Street = "12 rue de la Paix",
                City = "Paris",
                PostalCode = "75001",
                Country = "France"
            }
        };
        var patient2 = new Patient
        {
            FirstName = "Marie",
            LastName = "Curie",
            DateOfBirth = new DateTime(1990, 3, 22),
            FileNumber = "PAT-2024-002",
            Phone = "0600000002",
            Email = "marie.curie@email.com",
            Address = new Address
            {
                Street = "5 avenue Montaigne",
                City = "Lyon",
                PostalCode = "69001",
                Country = "France"
            }
        };
        var patient3 = new Patient
        {
            FirstName = "Thomas",
            LastName = "Leroy",
            DateOfBirth = new DateTime(1978, 11, 8),
            FileNumber = "PAT-2024-003",
            Phone = "0600000003",
            Email = "thomas.leroy@email.com",
            Address = new Address
            {
                Street = "8 rue Victor Hugo",
                City = "Marseille",
                PostalCode = "13001",
                Country = "France"
            }
        };

        context.Patients.AddRange(patient1, patient2, patient3);
        await context.SaveChangesAsync();

        // ---- Consultations ----
        context.Consultations.AddRange(
            new Consultation
            {
                PatientId = patient1.Id,
                DoctorId = doctor1.Id,
                Date = DateTime.Now.AddDays(2),
                Status = ConsultationStatus.Planned,
                Notes = "Bilan cardiaque annuel"
            },
            new Consultation
            {
                PatientId = patient2.Id,
                DoctorId = doctor2.Id,
                Date = DateTime.Now.AddDays(3),
                Status = ConsultationStatus.Planned,
                Notes = "Suivi neurologique"
            },
            new Consultation
            {
                PatientId = patient3.Id,
                DoctorId = doctor1.Id,
                Date = DateTime.Now.AddDays(-5),
                Status = ConsultationStatus.Completed,
                Notes = "Consultation de routine"
            },
            new Consultation
            {
                PatientId = patient1.Id,
                DoctorId = doctor2.Id,
                Date = DateTime.Now.AddDays(-10),
                Status = ConsultationStatus.Cancelled,
                Notes = "Annulée par le patient"
            }
        );

        await context.SaveChangesAsync();
    }
}