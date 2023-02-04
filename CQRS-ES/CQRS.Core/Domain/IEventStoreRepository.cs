﻿using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public interface IEventStoreRepository
{
    Task AppendAsync(EventModel @event);
    Task<List<EventModel>> FindByAggregateIdAsync(Guid aggregateId);
    Task<List<EventModel>> FindAllAsync();
}