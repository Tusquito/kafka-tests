using System.Text.Json;
using Kafka.Common.Events.Abstractions;

namespace Kafka.Common.Events.Get;

[KafkaEvent(EventKind.KEventGet)]
public class GetEvent : IEvent
{
    public required Guid EventId { get; init; } = Guid.NewGuid();
    public required Guid ResourceId { get; init; } = Guid.NewGuid();
    public required DateTime TimestampUtc { get; init; } = DateTime.UtcNow;

    public override string ToString()
    {
        return $"[{nameof(GetEvent)}] {JsonSerializer.Serialize(this)}";
    }
}