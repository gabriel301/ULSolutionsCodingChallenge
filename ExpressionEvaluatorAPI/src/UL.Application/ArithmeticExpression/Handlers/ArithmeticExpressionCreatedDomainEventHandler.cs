using MediatR;
using Microsoft.Extensions.Logging;
using UL.Application.Resources;
using UL.Domain.Events.ArithmeticExpression.Created;

namespace UL.Application.ArithmeticExpression.Handlers;
public class ArithmeticExpressionCreatedDomainEventHandler : INotificationHandler<ArithmeticExpressionCreatedEvent>
{
    private readonly ILogger<ArithmeticExpressionCreatedDomainEventHandler> _logger;

    public ArithmeticExpressionCreatedDomainEventHandler(ILogger<ArithmeticExpressionCreatedDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ArithmeticExpressionCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(LoggingTemplateResources.Event_Publised, notification);

        return Task.CompletedTask;
    }
}
