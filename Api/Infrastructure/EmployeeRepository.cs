using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Employee>> GetAll()
    {
        var employees = await _context.Employee
            .Include(e => e.Dependents)
            .ToListAsync();
        
        return employees;
    }
    
    public async Task<Employee?> GetById(int id)
    {
        return await _context.Employee
            .Include(e => e.Dependents)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee> Add([FromBody]Employee employee)
    {
         _context.Employee.Add(employee);
         await _context.SaveChangesAsync();
         
         return employee;
    }

    public async Task<Employee> Update(Employee employee)
    {
        _context.Employee.Update(employee);
        await _context.SaveChangesAsync();

        return employee;
    }
}