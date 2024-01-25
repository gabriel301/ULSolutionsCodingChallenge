using MediatR;
using Microsoft.Extensions.Logging;
using UL.Application.Resources;
using UL.Domain.Events.ExpressionTree.Evaluated;

namespace UL.Application.ExpressionTree.Handlers;
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
