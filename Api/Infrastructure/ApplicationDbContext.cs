using Api.Models;

namespace Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext:DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    
    public DbSet<Employee> Employee { get; set; }
    public DbSet<Dependent> Dependent { get; set; }
}