using MediatR;
using Microsoft.Extensions.Logging;
using UL.Application.Resources;
using UL.Domain.Events.ExpressionTree.Created;

namespace UL.Application.ExpressionTree.Handlers;
public sealed class ExpressionTreeCreatedDomainEventHandler : INotificationHandler<ExpressionTreeCreatedEvent>
{

    private readonly ILogger<ExpressionTreeCreatedDomainEventHandler> _logger;

    public ExpressionTreeCreatedDomainEventHandler(ILogger<ExpressionTreeCreatedDomainEventHandler> log)
    {
        _logger = log;
    }

    public Task Handle(ExpressionTreeCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(LoggingTemplateResources.Event_Publised, notification);

        return Task.CompletedTask;
    }
}
