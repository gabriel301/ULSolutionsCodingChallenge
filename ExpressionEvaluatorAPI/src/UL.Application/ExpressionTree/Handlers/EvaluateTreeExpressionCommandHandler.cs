using MediatR;
using UL.Application.Abstractions.Command;
using UL.Application.ExpressionTree.Command;
using UL.Domain.Entities.ExpressionTree;
using UL.Domain.Services.Abstraction;

namespace UL.Application.ExpressionTree.Handlers;
public sealed class EvaluateTreeExpressionCommandHandler : ICommandHandler<EvaluateTreeExpressionCommand, double>
{
    private readonly IPublisher _publisher;
    private readonly IOperationService _operationService;

    public EvaluateTreeExpressionCommandHandler(IPublisher publisher, IOperationService operationService)
    {
        _publisher = publisher;
        _operationService = operationService;
    }

    public async Task<double> Handle(EvaluateTreeExpressionCommand request, CancellationToken cancellationToken)
    {
        //Using block ensures dispose method will be called
        using (BinaryExpressionTree tree = BinaryExpressionTree.Create(request.expression))
        {
            double evaluationResult = 0.0d;
            try
            {
                evaluationResult = await Task.Run(() => tree.Evaluate(_operationService), cancellationToken);

            }
            finally
            {
                foreach (var item in tree.GetDomainEvents())
                {
                    await _publisher.Publish(item);
                }
                tree.ClearDomainEvents();
            }


            return evaluationResult;
        }
    }
}
