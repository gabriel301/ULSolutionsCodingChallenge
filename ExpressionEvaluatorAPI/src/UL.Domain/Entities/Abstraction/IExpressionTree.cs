using UL.Domain.Services.Abstraction;

namespace UL.Domain.Entities.Abstraction;
public interface IExpressionTree : IDisposable
{
    public double Evaluate(IOperationService operationService);
}
