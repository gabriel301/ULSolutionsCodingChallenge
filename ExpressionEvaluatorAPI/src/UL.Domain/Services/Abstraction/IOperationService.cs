namespace UL.Domain.Services.Abstraction;
public interface IOperationService
{
    double Calculate(string mathOperator, double operand1, double operand2);
}
