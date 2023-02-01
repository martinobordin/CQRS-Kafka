﻿using CQRS.Core.CQRS;

namespace CQRS.Core.Infrastructure;

public interface ICommandDispatcher
{
    void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand;
    Task SendAsync(BaseCommand command);
}
