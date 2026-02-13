using Mediator;
using Microsoft.Extensions.Logging;

namespace Kafka.Common.Events.Get;

public sealed class GetEventHandler(ILogger<GetEventHandler> logger) : INotificationHandler<GetEvent>
{
    public ValueTask Handle(GetEvent notification, CancellationToken cancellationToken)
    {
        logger.LogEventHandled(notification);
        return ValueTask.CompletedTask;
    }
}