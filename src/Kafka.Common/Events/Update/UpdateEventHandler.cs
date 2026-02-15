using Kafka.Common.Events.Get;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Kafka.Common.Events.Update;

public sealed class UpdateEventHandler(ILogger<GetEventHandler> logger) : ICommandHandler<UpdateEvent>
{
    public ValueTask<Unit> Handle(UpdateEvent notification, CancellationToken cancellationToken)
    {
        logger.LogEventHandled(notification);
        return Unit.ValueTask;
    }
}