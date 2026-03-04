using Microsoft.EntityFrameworkCore;
using HospitalManagement.Data;
using HospitalManagement.Models;
using HospitalManagement.Repositories;

namespace HospitalManagement.Tests;

public class PatientServiceTests
{
    // Fabrique un DbContext en mémoire isolé pour chaque test
    private HospitalDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<HospitalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new HospitalDbContext(options);
    }

    [Fact]
    public async Task AddPatient_ShouldPersistCorrectly()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new PatientRepository(context);

        var patient = new Patient
        {
            FirstName = "Jean",
            LastName = "Dupont",
            Email = "jean.dupont@test.com",
            Phone = "0600000000",
            FileNumber = "PAT-2024-001",
            DateOfBirth = new DateTime(1985, 6, 15),
            Address = new Address
            {
                Street = "1 rue de la Paix",
                City = "Paris",
                PostalCode = "75001",
                Country = "France"
            }
        };

        // Act
        var created = await repo.AddAsync(patient);

        // Assert
        Assert.NotNull(created);
        Assert.Equal("Dupont", created.LastName);
        Assert.Equal("PAT-2024-001", created.FileNumber);
        Assert.Equal(1, await context.Patients.CountAsync());
    }

    [Fact]
    public async Task SearchByName_ShouldReturnMatchingPatients()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new PatientRepository(context);

        await repo.AddAsync(new Patient
        {
            FirstName = "Jean",
            LastName = "Dupont",
            Email = "jean@test.com",
            Phone = "0600000001",
            FileNumber = "PAT-001",
            DateOfBirth = new DateTime(1985, 1, 1),
            Address = new Address()
        });

        await repo.AddAsync(new Patient
        {
            FirstName = "Marie",
            LastName = "Martin",
            Email = "marie@test.com",
            Phone = "0600000002",
            FileNumber = "PAT-002",
            DateOfBirth = new DateTime(1990, 1, 1),
            Address = new Address()
        });

        // Act
        var results = await repo.SearchByNameAsync("Dupont");

        // Assert
        Assert.Single(results);
        Assert.Equal("Jean", results.First().FirstName);
    }

    [Fact]
    public async Task DeletePatient_ShouldReturnFalse_WhenNotFound()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new PatientRepository(context);

        // Act
        var result = await repo.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }
}