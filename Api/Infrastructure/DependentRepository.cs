using Api.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure;

public class DependentRepository : IDependentRepository
{
    private readonly ApplicationDbContext _context;

    public DependentRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Dependent?> GetById(int id)
    {
        return await _context.Dependent
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public Task<Dependent> Update(Dependent dependent)
    {
        _context.Dependent.Update(dependent);
        return Task.FromResult(dependent);
    }

    public async Task<List<Dependent>> GetAll()
    {
        return await _context.Dependent
            .ToListAsync();
    }
}