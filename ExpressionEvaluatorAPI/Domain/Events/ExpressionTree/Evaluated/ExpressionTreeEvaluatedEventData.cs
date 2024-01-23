namespace Domain.Events.ExpressionTree.Evaluated;
public record class ExpressionTreeEvaluatedEventData(string expression, double evaluation)
{
}
