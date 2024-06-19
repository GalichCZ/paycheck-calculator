using Api.Models;

namespace Api.Dtos.Dependent;

public class GetDependentDto
{
    public int Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public DateTime DateOfBirth { get; init; }
    public Relationship Relationship { get; init; }
    
    public GetDependentDto() { }
    
    // used this constructor to make it easier to read in controllers
    public GetDependentDto(Models.Dependent dependent)
    {
        Id = dependent.Id;
        FirstName = dependent.FirstName;
        LastName = dependent.LastName;
        DateOfBirth = dependent.DateOfBirth;
        Relationship = dependent.Relationship;
    }
}
