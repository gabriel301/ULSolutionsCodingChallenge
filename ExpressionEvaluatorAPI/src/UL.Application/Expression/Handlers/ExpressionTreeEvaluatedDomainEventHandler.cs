using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UL.Application.Resources;
using UL.Domain.Events.ExpressionTree.Evaluated;

namespace UL.Application.Expression.Handlers;
public sealed class ExpressionTreeEvaluatedDomainEventHandler : INotificationHandler<ExpressionTreeEvaluatedEvent>
{
    private readonly ILogger<ExpressionTreeCreatedDomainEventHandler> _logger;

    public ExpressionTreeEvaluatedDomainEventHandler(ILogger<ExpressionTreeCreatedDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ExpressionTreeEvaluatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(LoggingTemplateResources.Event_Publised, notification);

        return Task.CompletedTask;
    }
}
