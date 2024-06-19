using Api.Models;

namespace Api.Interfaces;

public interface IDependentRepository
{
    Task<List<Dependent>> GetAll();
    
    Task<Dependent?> GetById(int id);
    
    Task<Dependent> Update(Dependent dependent);
}