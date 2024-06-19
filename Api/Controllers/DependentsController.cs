using Api.Dtos.Dependent;
using Api.Infrastructure;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DependentsController : ControllerBase
{
    private readonly DependentRepository _dependentRepository;

    public DependentsController(DependentRepository dependentRepository)
    {
        _dependentRepository = dependentRepository;
    }
    
    [SwaggerOperation(Summary = "Get dependent by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetDependentDto>>> Get(int id)
    {
        
        var dependent = await _dependentRepository.GetById(id);
        
        if(dependent == null) return NotFound();
        
        var dto = new GetDependentDto(dependent);
        
        return Ok(new ApiResponse<GetDependentDto>(dto));
    }

    [SwaggerOperation(Summary = "Get all dependents")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetDependentDto>>>> GetAll()
    {
        var dependents = await _dependentRepository.GetAll();

        var dtos = dependents
            .Select(d => new GetDependentDto(d))
            .ToList();

        return Ok(new ApiResponse<List<GetDependentDto>>(dtos));
    }
}
