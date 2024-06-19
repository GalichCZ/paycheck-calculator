using Api.Dtos.Dependent;

namespace Api.Dtos.Employee;

public class GetEmployeeDto
{
    public int Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public decimal Salary { get; init; }
    public DateTime DateOfBirth { get; init; }
    public ICollection<GetDependentDto> Dependents { get; init; } = new List<GetDependentDto>();
    
    public GetEmployeeDto() {}
    public GetEmployeeDto(Models.Employee employee)
    {
        Id = employee.Id;
        FirstName = employee.FirstName;
        LastName = employee.LastName;
        Salary = employee.Salary;
        DateOfBirth = employee.DateOfBirth;
        Dependents = employee.Dependents
            .Select(d => new GetDependentDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Relationship = d.Relationship,
                DateOfBirth = d.DateOfBirth
            })
            .ToList();
    }
}
