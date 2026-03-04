using HospitalManagement.Data;
using HospitalManagement.Services;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Base de données
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlite("Data Source=hospital.db"));

// Services métier
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IConsultationService, ConsultationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IConsultationRepository, ConsultationRepository>();

var app = builder.Build();

// Swagger uniquement en développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();