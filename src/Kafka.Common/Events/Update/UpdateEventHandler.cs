using Kafka.Common.Events.Get;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Kafka.Common.Events.Update;

public sealed class UpdateEventHandler(ILogger<GetEventHandler> logger) : INotificationHandler<UpdateEvent>
{
    public ValueTask Handle(UpdateEvent notification, CancellationToken cancellationToken)
    {
        logger.LogEventHandled(notification);
        return ValueTask.CompletedTask;
    }
}