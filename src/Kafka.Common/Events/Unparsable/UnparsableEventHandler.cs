using Mediator;

namespace Kafka.Common.Events.Unparsable;

public class UnparsableEventHandler : ICommandHandler<UnparsableEvent>
{
    public ValueTask<Unit> Handle(UnparsableEvent notification, CancellationToken cancellationToken)
    {
        throw new ArgumentException("Can not parse the message");
    }
}