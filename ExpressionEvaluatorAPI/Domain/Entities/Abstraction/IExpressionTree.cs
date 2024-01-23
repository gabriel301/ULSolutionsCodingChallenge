namespace Domain.Entities.Abstraction;
public interface IExpressionTree : IDisposable
{
    public double Evaluate();
}
