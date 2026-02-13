using System.Text.Json;
using Kafka.Common.Events.Abstractions;

namespace Kafka.Common.Events.Update;

[KafkaEvent(EventKind.K_EVENT_UPDATE)]
public class UpdateEvent : IEvent
{
    public required Guid EventId { get; init; } = Guid.NewGuid();
    public required Guid ResourceId { get; init; } = Guid.NewGuid();
    public required DateTime TimestampUtc { get; init; } = DateTime.UtcNow;

    public override string ToString()
    {
        return $"[{nameof(UpdateEvent)}] {JsonSerializer.Serialize(this)}";
    }
}