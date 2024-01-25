using UL.Domain.Services.Abstraction;

namespace UL.Application.DomainServices;
public class BasicArithmeticOperationService : IOperationService
{
    public double Calculate(string mathOperator, double operand1, double operand2) => mathOperator switch
    {
        "+" => operand1 + operand2,
        "-" => operand1 - operand2,
        "*" => operand1 * operand2,
        "/" => operand1 / operand2,
        _ => 0.0d,
    };
}
