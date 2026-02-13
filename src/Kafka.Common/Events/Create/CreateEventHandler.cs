using Mediator;
using Microsoft.Extensions.Logging;

namespace Kafka.Common.Events.Create;

public class CreateEventHandler(ILogger<CreateEventHandler> logger) : INotificationHandler<CreateEvent>
{
    public ValueTask Handle(CreateEvent notification, CancellationToken cancellationToken)
    {
        logger.LogEventHandled(notification);
        return ValueTask.CompletedTask;
    }
}