using EmployeeAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace EmployeeAPI.Infrastructure.Data;
public class EmployeeContext : DbContext
{
    public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options) { }
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(e => new { e.Name, e.Department })
                  .IsUnique()
                  .HasFilter("[IsActive] = 1");

            entity.Property(e => e.Salary)
                  .HasColumnType("decimal(18,2)");
        });
    }
}