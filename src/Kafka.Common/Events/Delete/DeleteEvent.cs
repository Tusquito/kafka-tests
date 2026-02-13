using System.Text.Json;
using Kafka.Common.Events.Abstractions;

namespace Kafka.Common.Events.Delete;

[KafkaEvent(EventKind.K_EVENT_DELETE)]
public sealed class DeleteEvent : IEvent
{
    public required Guid EventId { get; init; } = Guid.NewGuid();
    public required Guid ResourceId { get; init; } = Guid.NewGuid();
    public required DateTime TimestampUtc { get; init; } = DateTime.UtcNow;

    public override string ToString() => $"[{nameof(DeleteEvent)}] {JsonSerializer.Serialize(this)}";
}