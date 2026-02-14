using System.Text.Json;
using Kafka.Common.Events.Abstractions;

namespace Kafka.Common.Events.Delete;

[KafkaEvent(EventKind.KEventDelete)]
public sealed class DeleteEvent : IEvent
{
    public required Guid EventId { get; init; } = Guid.NewGuid();
    public required Guid ResourceId { get; init; } = Guid.NewGuid();
    public required DateTime TimestampUtc { get; init; } = DateTime.UtcNow;

    public override string ToString()
    {
        return $"[{nameof(DeleteEvent)}] {JsonSerializer.Serialize(this)}";
    }
}