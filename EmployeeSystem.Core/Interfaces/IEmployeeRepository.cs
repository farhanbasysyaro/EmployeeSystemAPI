using EmployeeAPI.Core.Entities;

namespace EmployeeAPI.Core.Interfaces;
public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id);
    Task<IEnumerable<Employee>> GetAllAsync(string? department, bool? isActive, string? sortBy);
    Task AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task SoftDeleteAsync(int id);
    Task<bool> ExistsAsync(string name, string department);
    Task<decimal> GetAverageSalaryAsync(string department);
}