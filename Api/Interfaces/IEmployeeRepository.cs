using Api.Models;

namespace Api.Interfaces;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAll();
    Task<Employee?> GetById(int id);
    Task<Employee> Add(Employee employee);
    Task<Employee> Update(Employee employee);
}