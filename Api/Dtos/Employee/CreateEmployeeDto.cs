using System.ComponentModel.DataAnnotations;
using Api.Dtos.Dependent;

namespace Api.Dtos.Employee;

public class CreateEmployeeDto
{
    // This validation saves us so much time on not implementing boilerplate code, like Validator classes -
    // reminds me Lombock for Java, even if Lombock is not validating :D
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
    public string? FirstName { get; init; }
    
    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
    public string? LastName { get; init; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number.")]
    public decimal Salary { get; init; }
    
    [Required(ErrorMessage = "Date of birth is required.")]
    public DateTime DateOfBirth { get; init; }
    
    public ICollection<GetDependentDto> Dependents { get; init; } = new List<GetDependentDto>();
}