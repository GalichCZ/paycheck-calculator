using Api.Interfaces;
using Api.Models;

namespace Api.Services;

public class PayChecksCalculator : IPayCheckCalculator
{
    private const decimal BaseBenefitsCost = 1000m;
    private const decimal CostPerDependent = 600m;
    private const decimal AdditionalElderDependentCost = 200m;
    private const decimal CostPerOverEarn = 0.02m;
    private const decimal OverEarnThreshold = 80000m;
    private const int NumberOfPayChecksPerYear = 26;

    public PayCheck CalculatePayCheck(Employee employee)
    {
        var dependents = employee.Dependents.ToList();
        
        const decimal benefitsAnnually = BaseBenefitsCost * 12;
        var dependentsDeduction = CalculateDependentsDeduction(dependents);
        var overEarnDeduction = CalculateOverEarn(employee.Salary);
        
        var totalDeduction = benefitsAnnually + dependentsDeduction + overEarnDeduction;
        
        var deductionsCostPerPayCheck = totalDeduction / NumberOfPayChecksPerYear;
        var grossPayPerPayCheck = employee.Salary / NumberOfPayChecksPerYear;
        var netPayPerPayCheck = grossPayPerPayCheck - deductionsCostPerPayCheck;
        
        return new PayCheck
        {
            GrossIncome = grossPayPerPayCheck,
            Deductions = deductionsCostPerPayCheck,
            NetIncome = netPayPerPayCheck
        };
        
    }
    
    private bool IsOver50(DateTime dateOfBirth)
    {
        var age = DateTime.Now.Year - dateOfBirth.Year;
        return age > 50;
    }

    private decimal CalculateOverEarn(decimal salary)
    {
        if (salary > OverEarnThreshold) return salary * CostPerOverEarn;
        return 0m;
    }
    
    private decimal CalculateDependentsDeduction(List<Dependent> dependents)
    {
        return dependents
            .Count(d => IsOver50(d.DateOfBirth))
            * CostPerDependent
            + dependents
                .Count(d => !IsOver50(d.DateOfBirth))
                * (AdditionalElderDependentCost + CostPerDependent);
    }
}