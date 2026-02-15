using System.Text.Json;
using System.Text.Json.Serialization;
using Kafka.Common.Events.Abstractions;

namespace Kafka.Common.Events.Create;

[KafkaEvent(EventKind.KEventCreate)]
public sealed class CreateEvent : IEvent
{
    public required Guid ResourceId { get; init; } = Guid.NewGuid();
    public required DateTime TimestampUtc { get; init; } = DateTime.UtcNow;

    [JsonIgnore] public Context Context { get; set; } = Context.Empty;

    public override string ToString()
    {
        return $"[{nameof(CreateEvent)}] {JsonSerializer.Serialize(this)}";
    }
}