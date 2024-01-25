using UL.Application.Abstractions.Command;

namespace UL.Application.ArithmeticExpression.Command;
public record EvaluateArithmeticExpressionCommand(string expression) : ICommand<double>
{
}
