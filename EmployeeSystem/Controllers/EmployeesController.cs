using EmployeeAPI.Core.DTOs;
using EmployeeAPI.Core.Entities;
using EmployeeAPI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _repository;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(
        IEmployeeRepository repository,
        ILogger<EmployeesController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? department,
        [FromQuery] bool? isActive,
        [FromQuery] string? sortBy)
    {
        try
        {
            var employees = await _repository.GetAllAsync(department, isActive, sortBy);
            return Ok(employees.Select(e => MapToDto(e)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employees");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EmployeeDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var employee = await _repository.GetByIdAsync(id);
            return employee == null ? NotFound() : Ok(MapToDto(employee));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving employee {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
    {
        try
        {
            var employee = new Employee
            {
                Name = dto.Name,
                Department = dto.Department,
                Salary = dto.Salary
            };

            await _repository.AddAsync(employee);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, MapToDto(employee));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        try
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var employee = await _repository.GetByIdAsync(id);
            if (employee == null) return NotFound();

            employee.Name = dto.Name;
            employee.Department = dto.Department;
            employee.Salary = dto.Salary;

            await _repository.UpdateAsync(employee);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating employee {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _repository.SoftDeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting employee {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("departments/{department}/average-salary")]
    [ProducesResponseType(typeof(decimal), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetAverageSalary(string department)
    {
        try
        {
            var average = await _repository.GetAverageSalaryAsync(department);
            return Ok(average);
        }
        catch (InvalidOperationException)
        {
            return NotFound("Department not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting average salary for {department}");
            return StatusCode(500, "Internal server error");
        }
    }

    private static EmployeeDto MapToDto(Employee employee) => new(
        employee.Id,
        employee.Name,
        employee.Department,
        employee.Salary,
        employee.IsActive
    );
}