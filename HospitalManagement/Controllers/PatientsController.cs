using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Models;
using HospitalManagement.Services;

namespace HospitalManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _service;

    public PatientsController(IPatientService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(await _service.GetAllAsync(page, pageSize));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var patient = await _service.GetByIdAsync(id);
        return patient is null ? NotFound() : Ok(patient);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string name)
        => Ok(await _service.SearchByNameAsync(name));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Patient patient)
    {
        var created = await _service.CreateAsync(patient);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Patient patient)
    {
        var updated = await _service.UpdateAsync(id, patient);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}