using MediatR;
using UL.Application.Abstractions.Command;
using UL.Application.ArithmeticExpression.Command;
using UL.Domain.Entities.ArithmeticExpression;
using UL.Domain.Services.Abstraction;

namespace UL.Application.ArithmeticExpression.Handlers;
public class EvaluateArithmeticExpressionCommandHandler : ICommandHandler<EvaluateArithmeticExpressionCommand, double>
{

    private readonly IPublisher _publisher;
    private readonly IOperationService _operationService;

    public EvaluateArithmeticExpressionCommandHandler(IPublisher publisher, IOperationService operationService)
    {
        _publisher = publisher;
        _operationService = operationService;
    }

    public async Task<double> Handle(EvaluateArithmeticExpressionCommand request, CancellationToken cancellationToken)
    {
        double result = 0.0;

        ArithmeticExpressionEvaluator expression = ArithmeticExpressionEvaluator.Create(request.expression);

        try
        {
            result = expression.Evaluate(_operationService);
        }
        finally
        {
            foreach (var item in expression.GetDomainEvents())
            {
                await _publisher.Publish(item);
            }
            expression.ClearDomainEvents();
        }

        return result;
    }
}
