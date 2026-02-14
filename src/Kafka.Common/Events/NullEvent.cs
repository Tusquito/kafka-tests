using Confluent.Kafka;
using Kafka.Common.Events.Abstractions;

namespace Kafka.Common.Events;

public record NullEvent(SerializationContext Context) : IEvent
{
    public static readonly NullEvent Null = new(SerializationContext.Empty);
}