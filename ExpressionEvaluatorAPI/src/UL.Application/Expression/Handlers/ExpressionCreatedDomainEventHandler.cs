﻿using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UL.Application.Resources;
using UL.Domain.Events.ExpressionTree.Created;

namespace UL.Application.Expression.Handlers;
public sealed class ExpressionCreatedDomainEventHandler : INotificationHandler<ExpressionTreeCreatedEvent>
{

    private readonly ILogger<ExpressionCreatedDomainEventHandler> _logger;

    public ExpressionCreatedDomainEventHandler(ILogger<ExpressionCreatedDomainEventHandler> log)
    {
        _logger = log;
    }

    public Task Handle(ExpressionTreeCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(LoggingTemplateResources.Event_Publised, notification);

        return Task.CompletedTask;
    }
}