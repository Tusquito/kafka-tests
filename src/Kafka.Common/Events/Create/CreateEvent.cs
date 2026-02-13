using System.Text.Json;
using Kafka.Common.Events.Abstractions;

namespace Kafka.Common.Events.Create;

[KafkaEvent(EventKind.K_EVENT_CREATE)]
public sealed class CreateEvent : IEvent
{
    public required Guid EventId { get; init; } = Guid.NewGuid();
    public required Guid ResourceId { get; init; } = Guid.NewGuid();
    public required DateTime TimestampUtc { get; init; } = DateTime.UtcNow;

    public override string ToString()
    {
        return $"[{nameof(CreateEvent)}] {JsonSerializer.Serialize(this)}";
    }
}