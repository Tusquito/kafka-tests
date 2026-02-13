using Mediator;
using Microsoft.Extensions.Logging;

namespace Kafka.Common.Events.Delete;

public class DeleteEventHandler(ILogger<DeleteEventHandler> logger) : INotificationHandler<DeleteEvent>
{
    public ValueTask Handle(DeleteEvent notification, CancellationToken cancellationToken)
    {
        logger.LogEventHandled(notification);
        return ValueTask.CompletedTask;
    }
}