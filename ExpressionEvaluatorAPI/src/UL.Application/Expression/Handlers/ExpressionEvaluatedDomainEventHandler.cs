using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UL.Domain.Events.ExpressionTree.Evaluated;

namespace UL.Application.Expression.Handlers;
internal sealed class ExpressionEvaluatedDomainEventHandler : INotificationHandler<ExpressionTreeEvaluatedEvent>
{
    private readonly ILogger _logger;

    public ExpressionEvaluatedDomainEventHandler(ILogger logger)
    {
        _logger = logger;
    }

    public Task Handle(ExpressionTreeEvaluatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Teste", notification.UTCEventDateTime, notification.EventData.expression,notification.EventData.evaluation);

        return Task.CompletedTask;
    }
}
