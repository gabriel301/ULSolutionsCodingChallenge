using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UL.Application.Abstractions.Command;
using UL.Application.Expression.Command;
using UL.Entities.Expression;

namespace UL.Application.Expression.Handlers;
public sealed class EvaluateExpressionCommandHandler : ICommandHandler<EvaluateExpressionCommand, double>
{
    private readonly IPublisher _publisher;

    public EvaluateExpressionCommandHandler(IPublisher publisher)
    {
        this._publisher = publisher;
    }

    public async Task<double> Handle(EvaluateExpressionCommand request, CancellationToken cancellationToken)
    {
        //Using block ensures dispose method will be called
        using (ExpressionTree tree = ExpressionTree.Create(request.expression)) 
        {
            var evaluationResult = await Task.Run(() => tree.Evaluate(), cancellationToken);

            foreach (var item in tree.GetDomainEvents())
            {
                await _publisher.Publish(item);
            }

            tree.ClearDomainEvents();
            return evaluationResult;
        }
    }
}
