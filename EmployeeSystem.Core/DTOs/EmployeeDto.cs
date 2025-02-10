using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.Core.DTOs;
public record EmployeeDto(
    int Id,
    string Name,
    string Department,
    decimal Salary,
    bool IsActive
);

public record CreateEmployeeDto(
    string Name,
    string Department,
    [Range(0, 1_000_000)] decimal Salary
);

public record UpdateEmployeeDto(
    int Id,
    string Name,
    string Department,
    [Range(0, 1_000_000)] decimal Salary
);