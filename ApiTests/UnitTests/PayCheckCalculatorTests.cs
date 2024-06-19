using System;
using System.Collections.Generic;
using Api.Models;
using Api.Services;
using Xunit;

namespace ApiTests.UnitTests;

public class PayCheckCalculatorTests
{
    private readonly PayChecksCalculator _payChecksCalculator = new PayChecksCalculator();
    
    [Fact]
    public void CalculatePayCheck_NoDependents_UnderThreshold()
    {
        var employee = new Employee
        {
            Salary = 75000m,
            Dependents = new List<Dependent>()
        };

        var paycheck = _payChecksCalculator.CalculatePayCheck(employee);

        Assert.Equal(employee.Salary / 26, paycheck.GrossIncome);
        Assert.Equal((1000m * 12) / 26, paycheck.Deductions);
        Assert.Equal(paycheck.GrossIncome - paycheck.Deductions, paycheck.NetIncome);
    }
    
    [Fact]
    public void CalculatePayCheck_NoDependents_OverThreshold()
    {
        var employee = new Employee
        {
            Salary = 90000m,
            Dependents = new List<Dependent>()
        };

        var paycheck = _payChecksCalculator.CalculatePayCheck(employee);

        var expectedDeductions = ((1000m * 12) + (employee.Salary * 0.02m)) / 26;
        Assert.Equal(employee.Salary / 26, paycheck.GrossIncome);
        Assert.Equal(expectedDeductions, paycheck.Deductions);
        Assert.Equal(paycheck.GrossIncome - paycheck.Deductions, paycheck.NetIncome);
    }
    
    [Fact]
    public void CalculatePayCheck_WithDependents()
    {
        var employee = new Employee
        {
            Salary = 75000m,
            Dependents = new List<Dependent>
            {
                new Dependent { DateOfBirth = DateTime.Now.AddYears(-30) },
                new Dependent { DateOfBirth = DateTime.Now.AddYears(-55) }
            }
        };

        var paycheck = _payChecksCalculator.CalculatePayCheck(employee);

        var expectedDependentsDeduction = (600m + 200m) + 600m;
        var expectedDeductions = ((1000m * 12) + expectedDependentsDeduction) / 26;
        Assert.Equal(employee.Salary / 26, paycheck.GrossIncome);
        Assert.Equal(expectedDeductions, paycheck.Deductions);
        Assert.Equal(paycheck.GrossIncome - paycheck.Deductions, paycheck.NetIncome);
    }
    
    [Fact]
    public void CalculatePayCheck_WithDependents_OverThreshold()
    {
        var employee = new Employee
        {
            Salary = 90000m,
            Dependents = new List<Dependent>
            {
                new Dependent { DateOfBirth = DateTime.Now.AddYears(-30) },
                new Dependent { DateOfBirth = DateTime.Now.AddYears(-55) }
            }
        };

        var paycheck = _payChecksCalculator.CalculatePayCheck(employee);

        var expectedDependentsDeduction = (600m + 200m) + 600m;
        var expectedDeductions = ((1000m * 12) + expectedDependentsDeduction + (employee.Salary * 0.02m)) / 26;
        Assert.Equal(employee.Salary / 26, paycheck.GrossIncome);
        Assert.Equal(expectedDeductions, paycheck.Deductions);
        Assert.Equal(paycheck.GrossIncome - paycheck.Deductions, paycheck.NetIncome);
    }
}