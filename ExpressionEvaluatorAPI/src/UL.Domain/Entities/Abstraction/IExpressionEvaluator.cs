using UL.Domain.Services.Abstraction;

namespace UL.Domain.Entities.Abstraction;
public interface IExpressionEvaluator
{
    public double Evaluate(IOperationService operationService);
}
