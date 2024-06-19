using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Dtos.Dependent;
using Api.Infrastructure;
using Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ApiTests.IntegrationTests;

public class DependentIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private HttpClient _httpClient;
    
    public DependentIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
    [Fact]
    public async Task WhenAskedForAllDependents_ShouldReturnAllDependents()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var db = scopedService.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureCreated();
            db.Database.Migrate();
            Seeding.InitializeTestDb(db);
        }
        
        var response = await _httpClient.GetAsync("/api/v1/dependents");
        var dependents = new List<GetDependentDto>
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
            },
            new()
            {
                Id = 4,
                FirstName = "DP",
                LastName = "Jordan",
                Relationship = Relationship.DomesticPartner,
                DateOfBirth = new DateTime(1974, 1, 2)
            }
        };
        await response.ShouldReturn(HttpStatusCode.OK, dependents);
    }

    [Fact]
    public async Task WhenAskedForADependent_ShouldReturnCorrectDependent()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedService = scope.ServiceProvider;
            var db = scopedService.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureCreated();
            db.Database.Migrate();
        }
        
        var response = await _httpClient.GetAsync("/api/v1/dependents/1");
        var dependent = new GetDependentDto
        {
            Id = 1,
            FirstName = "Spouse",
            LastName = "Morant",
            Relationship = Relationship.Spouse,
            DateOfBirth = new DateTime(1998, 3, 3)
        };
        await response.ShouldReturn(HttpStatusCode.OK, dependent);
    }

    [Fact]
    //task: make test pass
    public async Task WhenAskedForANonexistentDependent_ShouldReturn404()
    {
        var response = await _httpClient.GetAsync($"/api/v1/dependents/{int.MinValue}");
        await response.ShouldReturn(HttpStatusCode.NotFound);
    }
}

