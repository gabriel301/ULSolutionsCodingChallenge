﻿using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UL.Application.Resources;
using UL.Domain.Events.ExpressionTree.Evaluated;

namespace UL.Application.Expression.Handlers;
public sealed class ExpressionEvaluatedDomainEventHandler : INotificationHandler<ExpressionTreeEvaluatedEvent>
{
    private readonly ILogger<ExpressionCreatedDomainEventHandler> _logger;

    public ExpressionEvaluatedDomainEventHandler(ILogger<ExpressionCreatedDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ExpressionTreeEvaluatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(LoggingTemplateResources.Event_Publised, notification);

        return Task.CompletedTask;
    }
}