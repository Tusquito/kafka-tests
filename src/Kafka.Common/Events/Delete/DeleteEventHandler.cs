using Mediator;
using Microsoft.Extensions.Logging;

namespace Kafka.Common.Events.Delete;

public class DeleteEventHandler(ILogger<DeleteEventHandler> logger) : ICommandHandler<DeleteEvent>
{
    public ValueTask<Unit> Handle(DeleteEvent notification, CancellationToken cancellationToken)
    {
        logger.LogEventHandled(notification);
        return Unit.ValueTask;
    }
}