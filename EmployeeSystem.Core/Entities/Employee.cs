namespace EmployeeAPI.Core.Entities;
public class Employee
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Department { get; set; }
    public decimal Salary { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}