using MediatR;
using Microsoft.Extensions.Logging;
using UL.Application.Resources;
using UL.Domain.Events.ArithmeticExpression.Evaluated;

namespace UL.Application.ArithmeticExpression.Handlers;
public class ArithmeticExpressionEvaluatedDomainEventHandler : INotificationHandler<ArithmeticExpressionEvaluatedEvent>
{
    private readonly ILogger<ArithmeticExpressionCreatedDomainEventHandler> _logger;

    public ArithmeticExpressionEvaluatedDomainEventHandler(ILogger<ArithmeticExpressionCreatedDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ArithmeticExpressionEvaluatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(LoggingTemplateResources.Event_Publised, notification);

        return Task.CompletedTask;
    }
}
