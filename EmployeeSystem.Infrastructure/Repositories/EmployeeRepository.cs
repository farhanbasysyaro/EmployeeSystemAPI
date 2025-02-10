using EmployeeAPI.Core.Entities;
using EmployeeAPI.Core.Interfaces;
using EmployeeAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAPI.Infrastructure.Repositories;
public class EmployeeRepository : IEmployeeRepository
{
    private readonly EmployeeContext _context;

    public EmployeeRepository(EmployeeContext context) => _context = context;

    public async Task<Employee?> GetByIdAsync(int id)
        => await _context.Employees.FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

    public async Task<IEnumerable<Employee>> GetAllAsync(string? department, bool? isActive, string? sortBy)
    {
        var query = _context.Employees.AsQueryable();

        if (!string.IsNullOrEmpty(department))
            query = query.Where(e => e.Department == department);

        if (isActive.HasValue)
            query = query.Where(e => e.IsActive == isActive.Value);

        query = sortBy?.ToLower() switch
        {
            "name" => query.OrderBy(e => e.Name),
            "salary" => query.OrderBy(e => e.Salary),
            _ => query.OrderBy(e => e.Id)
        };

        return await query.ToListAsync();
    }

    public async Task AddAsync(Employee employee)
    {
        if (await _context.Employees.AnyAsync(e =>
            e.Name == employee.Name &&
            e.Department == employee.Department))
            throw new InvalidOperationException("Employee name must be unique within department");

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Employee employee)
    {
        if (await _context.Employees.AnyAsync(e =>
            e.Id != employee.Id &&
            e.Name == employee.Name &&
            e.Department == employee.Department))
            throw new InvalidOperationException("Employee name must be unique within department");

        employee.UpdatedAt = DateTime.UtcNow;
        _context.Entry(employee).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var employee = await GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Employee not found");

        employee.IsActive = false;
        await UpdateAsync(employee);
    }

    public async Task<bool> ExistsAsync(string name, string department)
        => await _context.Employees.AnyAsync(e =>
            e.Name == name &&
            e.Department == department &&
            e.IsActive);

    public async Task<decimal> GetAverageSalaryAsync(string department)
        => await _context.Employees
            .Where(e => e.Department == department && e.IsActive)
            .AverageAsync(e => e.Salary);
}