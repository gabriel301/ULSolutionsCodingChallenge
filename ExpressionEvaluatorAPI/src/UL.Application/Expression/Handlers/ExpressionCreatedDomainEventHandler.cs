using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UL.Domain.Events.ExpressionTree.Created;

namespace UL.Application.Expression.Handlers;
internal sealed class ExpressionCreatedDomainEventHandler : INotificationHandler<ExpressionTreeCreatedEvent>
{

    private readonly ILogger _logger;

    public ExpressionCreatedDomainEventHandler(ILogger log)
    {
        _logger = log;
    }

    public Task Handle(ExpressionTreeCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Teste", notification.UTCEventDateTime, notification.EventData.expression);

        return Task.CompletedTask;
    }
}
