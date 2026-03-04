using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Data;
using HospitalManagement.Models;
using HospitalManagement.DTOs;
using HospitalManagement.Services;

namespace HospitalManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultationsController : ControllerBase
{
    private readonly HospitalDbContext _context;
    private readonly IConsultationService _service;

    public ConsultationsController(HospitalDbContext context, IConsultationService service)
    {
        _context = context;
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Schedule([FromBody] CreateConsultationDto dto)
    {
        var consultation = new Consultation
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            Date = dto.Date,
            Status = (ConsultationStatus)dto.Status,
            Notes = dto.Notes
        };

        var created = await _service.ScheduleAsync(consultation);
        return CreatedAtAction(nameof(Schedule), new { id = created.Id }, created);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] ConsultationStatus status)
    {
        var updated = await _service.UpdateStatusAsync(id, status);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _service.CancelAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("patient/{patientId}/upcoming")]
    public async Task<IActionResult> GetUpcomingForPatient(int patientId)
        => Ok(await _service.GetUpcomingForPatientAsync(patientId));

    [HttpGet("doctor/{doctorId}/today")]
    public async Task<IActionResult> GetTodayForDoctor(int doctorId)
        => Ok(await _service.GetTodayForDoctorAsync(doctorId));
}