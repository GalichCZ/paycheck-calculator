using Api.Dtos.Employee;
using Api.Infrastructure;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

//TODO: add EP that will create and add dependent if it doesn't exist
[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly PayChecksCalculator _payChecksCalculator;
    private readonly EmployeeRepository _employeeRepository;
    private readonly DependentRepository _dependentRepository;

    public EmployeesController(PayChecksCalculator payChecksCalculator, EmployeeRepository employeeRepository, DependentRepository dependentRepository)
    {
        _payChecksCalculator = payChecksCalculator;
        _employeeRepository = employeeRepository;
        _dependentRepository = dependentRepository;
    }

    [SwaggerOperation(Summary = "Add dependent to employee")]
    [HttpPut("{employeeId}/{dependentId}/dependents")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> AddDependent(int employeeId, int dependentId)
    {
        var dependent = await _dependentRepository.GetById(dependentId);
        if (dependent == null)
        {
            return NotFound("Dependent not found");
        }
        
        var employee = await _employeeRepository.GetById(employeeId);
        if (employee == null)
        {
            return NotFound("Employee not found");
        }
        
        if (employee.Dependents.Any(d => d.Id == dependent.Id))
        {
            return BadRequest($"Dependent with ID {dependent.Id} already exists for employee with ID {employeeId}.");
        }
             
        var hasSpouse = employee.Dependents.Any(d => d.Relationship == Relationship.Spouse);
        var hasDomesticPartner = employee.Dependents.Any(d => d.Relationship == Relationship.DomesticPartner);
        var newDependentIsSpouseOrDomesticPartner = dependent.Relationship is Relationship.Spouse or Relationship.DomesticPartner;

        if ((hasSpouse || hasDomesticPartner) && newDependentIsSpouseOrDomesticPartner)
        {
            return BadRequest($"Employee with ID {employeeId} already has a spouse or domestic partner.");
        }
             
        dependent.EmployeeId = employeeId;
        dependent.Employee = employee;
             
        employee.Dependents.Add(dependent);
             
        await _employeeRepository.Update(employee);
        await _dependentRepository.Update(dependent);
             
        return Ok(new ApiResponse<GetEmployeeDto>(new GetEmployeeDto(employee)));
    }
    
    [SwaggerOperation(Summary = "Create new employee")]
    [HttpPost("")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Create([FromBody] CreateEmployeeDto createEmployeeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var newEmployee = new Employee
        {
            FirstName = createEmployeeDto.FirstName,
            LastName = createEmployeeDto.LastName,
            DateOfBirth = createEmployeeDto.DateOfBirth,
            Salary = createEmployeeDto.Salary
        };
        
        var employee = await _employeeRepository.Add(newEmployee);
        
        return Ok(new ApiResponse<GetEmployeeDto>(new GetEmployeeDto(employee)));
    }

    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id)
    {
        var employee = await _employeeRepository.GetById(id);

        if (employee == null) return NotFound();
        
        var dto = new GetEmployeeDto(employee);
        
        return Ok(new ApiResponse<GetEmployeeDto>(dto));
    }

    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll()
    {
        var employees = await _employeeRepository.GetAll();

        var dtos = employees
            .Select(e => new GetEmployeeDto(e)).ToList();

        return Ok(new ApiResponse<List<GetEmployeeDto>>(dtos));
    }

    [SwaggerOperation(Summary = "Get calculated paycheck")]
    [HttpGet("{id}/paycheck")]
    public async Task<ActionResult<ApiResponse<GetPayCheckDto>>> GetPayCheck(int id)
    {
        var employee = await _employeeRepository.GetById(id);

        if (employee == null) return NotFound();
        
        var payCheck = _payChecksCalculator.CalculatePayCheck(employee);
        
        var dto = new GetPayCheckDto(payCheck);
        
        return Ok(new ApiResponse<GetPayCheckDto>(dto));
    }
}
