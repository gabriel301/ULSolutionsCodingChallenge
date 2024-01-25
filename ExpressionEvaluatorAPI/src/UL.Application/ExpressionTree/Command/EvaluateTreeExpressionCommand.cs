using UL.Application.Abstractions.Command;

namespace UL.Application.ExpressionTree.Command;
public record EvaluateTreeExpressionCommand(string expression) : ICommand<double>;
