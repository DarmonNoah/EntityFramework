using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Data;
using HospitalManagement.Models;

namespace HospitalManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly HospitalDbContext _context;

    public DoctorsController(HospitalDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _context.Doctors.AsNoTracking().Include(d => d.Department).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var doctor = await _context.Doctors
            .Include(d => d.Department)
            .FirstOrDefaultAsync(d => d.Id == id);
        return doctor is null ? NotFound() : Ok(doctor);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Doctor doctor)
    {
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, doctor);
    }
}