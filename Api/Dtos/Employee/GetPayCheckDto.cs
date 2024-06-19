using Api.Models;

namespace Api.Dtos.Employee;

public class GetPayCheckDto
{
    public decimal GrossIncome { get; init; }
    public decimal Deductions { get; init; }
    public decimal NetIncome { get; init; }

    public GetPayCheckDto(PayCheck payCheck)
    {
        GrossIncome = decimal.Round(payCheck.GrossIncome, 2);
        Deductions = decimal.Round(payCheck.Deductions, 2);
        NetIncome = decimal.Round(payCheck.NetIncome, 2);
    }
}