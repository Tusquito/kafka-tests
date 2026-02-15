using Mediator;

namespace Kafka.Common.Events.Null;

public class NullEventHandler : ICommandHandler<NullEvent>
{
    public ValueTask<Unit> Handle(NullEvent notification, CancellationToken cancellationToken)
    {
        return Unit.ValueTask;
    }
}