using Mediator;
using Microsoft.Extensions.Logging;

namespace Kafka.Common.Events.Create;

public class CreateEventHandler(ILogger<CreateEventHandler> logger) : ICommandHandler<CreateEvent>
{
    public ValueTask<Unit> Handle(CreateEvent notification, CancellationToken cancellationToken)
    {
        logger.LogEventHandled(notification);

        // Test exception for retry testing
        throw new InvalidOperationException("An error occured while handling a create event");
    }
}