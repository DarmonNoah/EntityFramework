using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Services;

namespace HospitalManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    // GET api/dashboard/patients/1
    [HttpGet("patients/{patientId}")]
    public async Task<IActionResult> GetPatientDetails(int patientId)
    {
        var result = await _service.GetPatientDetailsAsync(patientId);
        return result is null ? NotFound() : Ok(result);
    }

    // GET api/dashboard/doctors/1/planning
    [HttpGet("doctors/{doctorId}/planning")]
    public async Task<IActionResult> GetDoctorPlanning(int doctorId)
    {
        var result = await _service.GetDoctorPlanningAsync(doctorId);
        return result is null ? NotFound() : Ok(result);
    }

    // GET api/dashboard/departments/stats
    [HttpGet("departments/stats")]
    public async Task<IActionResult> GetDepartmentStats()
    {
        var result = await _service.GetDepartmentStatsAsync();
        return Ok(result);
    }
}