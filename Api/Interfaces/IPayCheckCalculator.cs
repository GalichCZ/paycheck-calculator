using Api.Models;

namespace Api.Interfaces;

public interface IPayCheckCalculator
{
    PayCheck CalculatePayCheck(Employee employee);
}