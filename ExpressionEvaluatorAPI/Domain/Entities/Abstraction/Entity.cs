﻿using Shared.Events;

namespace Domain.Entities.Abstraction;
public abstract class Entity
{
    private readonly List<IEvent> _events = new();

    public virtual IReadOnlyList<IEvent> GetDomainEvents()
    {
        return _events.AsReadOnly();
    }

    public virtual void ClearDomainEvents()
    {
        _events.Clear();
    }

    protected virtual void AddDomainEvent(IEvent domainEvent)
    {
        _events.Add(domainEvent);
    }
}
