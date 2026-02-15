using Mediator;
using Microsoft.Extensions.Logging;

namespace Kafka.Common.Events.Get;

public sealed class GetEventHandler(ILogger<GetEventHandler> logger) : ICommandHandler<GetEvent>
{
    public ValueTask<Unit> Handle(GetEvent notification, CancellationToken cancellationToken)
    {
        logger.LogEventHandled(notification);
        return Unit.ValueTask;
    }
}