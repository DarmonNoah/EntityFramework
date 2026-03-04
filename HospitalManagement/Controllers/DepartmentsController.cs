using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Data;
using HospitalManagement.Models;

namespace HospitalManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly HospitalDbContext _context;

    public DepartmentsController(HospitalDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _context.Departments.AsNoTracking().ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dept = await _context.Departments.FindAsync(id);
        return dept is null ? NotFound() : Ok(dept);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Department department)
    {
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
    }
}