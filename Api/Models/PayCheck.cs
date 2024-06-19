namespace Api.Models;

public class PayCheck
{
    public decimal GrossIncome { get; init; }
    public decimal Deductions { get; init; }
    public decimal NetIncome { get; init; }
}