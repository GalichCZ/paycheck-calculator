using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Infrastructure;
using Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace ApiTests.IntegrationTests;

public class EmployeeIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private HttpClient _httpClient;
    
    public EmployeeIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
    
    [Fact]
    public async Task WhenAskedForAllEmployees_ShouldReturnAllEmployees()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var db = scopedService.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureCreated();
            db.Database.Migrate();
            Seeding.InitializeTestDb(db);
        }
        
        var response = await _httpClient.GetAsync("/api/v1/employees");
        var employees = new List<GetEmployeeDto>
        {
            new()
            {
                Id = 1,
                FirstName = "LeBron",
                LastName = "James",
                Salary = 75420.99m,
                DateOfBirth = new DateTime(1984, 12, 30)
            },
            new()
            {
                Id = 2,
                FirstName = "Ja",
                LastName = "Morant",
                Salary = 92365.22m,
                DateOfBirth = new DateTime(1999, 8, 10),
                Dependents = new List<GetDependentDto>
                {
                    new()
                    {
                        Id = 1,
                        FirstName = "Spouse",
                        LastName = "Morant",
                        Relationship = Relationship.Spouse,
                        DateOfBirth = new DateTime(1998, 3, 3)
                    },
                    new()
                    {
                        Id = 2,
                        FirstName = "Child1",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2020, 6, 23)
                    },
                    new()
                    {
                        Id = 3,
                        FirstName = "Child2",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2021, 5, 18)
                    }
                }
            },
            new()
            {
                Id = 3,
                FirstName = "Michael",
                LastName = "Jordan",
                Salary = 143211.12m,
                DateOfBirth = new DateTime(1963, 2, 17),
                Dependents = new List<GetDependentDto>
                {
                    new()
                    {
                        Id = 4,
                        FirstName = "DP",
                        LastName = "Jordan",
                        Relationship = Relationship.DomesticPartner,
                        DateOfBirth = new DateTime(1974, 1, 2)
                    }
                }
            }
        };
        await response.ShouldReturn(HttpStatusCode.OK, employees);
    }

    [Fact]
    //task: make test pass
    public async Task WhenAskedForAnEmployee_ShouldReturnCorrectEmployee()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var db = scopedService.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureCreated();
            db.Database.Migrate();
        }
        
        var response = await _httpClient.GetAsync("/api/v1/employees/1");
        var employee = new GetEmployeeDto
        {
            Id = 1,
            FirstName = "LeBron",
            LastName = "James",
            Salary = 75420.99m,
            DateOfBirth = new DateTime(1984, 12, 30)
        };
        await response.ShouldReturn(HttpStatusCode.OK, employee);
    }
    
    [Fact]
    //task: make test pass
    public async Task WhenAskedForANonexistentEmployee_ShouldReturn404()
    {
    var response = await _httpClient.GetAsync($"/api/v1/employees/{int.MinValue}");
        await response.ShouldReturn(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task WhenCreatingANewEmployee_ShouldReturnCreatedEmployee()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var db = scopedService.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureCreated();
            db.Database.Migrate();
        }
        
        var newEmployee = new CreateEmployeeDto
        {
            FirstName = "New",
            LastName = "Employee",
            Salary = 50000.00m,
            DateOfBirth = new DateTime(1990, 1, 1),
            Dependents = new List<GetDependentDto>()
        };

        var content = new StringContent(JsonConvert.SerializeObject(newEmployee), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/v1/employees", content);

        response.EnsureSuccessStatusCode();

        var createdEmployee = JsonConvert.DeserializeObject<ApiResponse<GetEmployeeDto>>(await response.Content.ReadAsStringAsync());

        Assert.NotNull(createdEmployee);
        Assert.Equal(newEmployee.FirstName, createdEmployee.Data.FirstName);
        Assert.Equal(newEmployee.LastName, createdEmployee.Data.LastName);
        Assert.Equal(newEmployee.Salary, createdEmployee.Data.Salary);
        Assert.Equal(newEmployee.DateOfBirth, createdEmployee.Data.DateOfBirth);
    }
    
    [Fact]
    public async Task WhenCreatingEmployeeWithInvalidData_ShouldReturnBadRequest()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var db = scopedService.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureCreated();
            db.Database.Migrate();
        }
        
        var invalidEmployee = new CreateEmployeeDto
        {
            // Missing required fields
            Salary = -50000.00m, // Invalid salary
            DateOfBirth = new DateTime(1990, 1, 1),
            Dependents = new List<GetDependentDto>()
        };

        var content = new StringContent(JsonConvert.SerializeObject(invalidEmployee), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/v1/employees", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task WhenAddingDependentToEmployee_ShouldReturnUpdatedEmployee()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var db = scopedService.GetRequiredService<ApplicationDbContext>();
            
            db.Database.EnsureCreated();
            db.Database.Migrate();
            
            db.Dependent.Add(new Dependent
            {
                Id = 101,
                FirstName = "a",
                LastName = "b",
                Relationship = Relationship.DomesticPartner,
                DateOfBirth = new DateTime(2000, 1, 1)
            });

            db.SaveChanges();
        }
        
        var response = await _httpClient.PutAsync($"/api/v1/employees/1/101/dependents", null);

        response.EnsureSuccessStatusCode();

        var updatedEmployee = JsonConvert.DeserializeObject<ApiResponse<GetEmployeeDto>>(await response.Content.ReadAsStringAsync());

        Assert.NotNull(updatedEmployee);
        Assert.Equal(1, updatedEmployee.Data.Id);
        Assert.Contains(updatedEmployee.Data.Dependents, d => d.Id == 101);
    }

    [Fact]
    public async Task WhenAddingExistingDependentToEmployee_ShouldReturnBadRequest()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var db = scopedService.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureCreated();
            db.Database.Migrate();
        }

        var response = await _httpClient.PutAsync($"/api/v1/employees/2/1/dependents", null);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task WhenAddingDependentToNonexistentEmployee_ShouldReturnNotFound()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var db = scopedService.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureCreated();
            db.Database.Migrate();
        }
        
        var response = await _httpClient.PutAsync($"/api/v1/employees/2000/1/dependents", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task WhenAddingNonexistentDependentToEmployee_ShouldReturnNotFound()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var db = scopedService.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureCreated();
            db.Database.Migrate();
        }
        
        var response = await _httpClient.PutAsync($"/api/v1/employees/1/1000/dependents", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

