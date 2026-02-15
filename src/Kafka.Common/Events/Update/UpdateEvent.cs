using System.Text.Json;
using System.Text.Json.Serialization;
using Kafka.Common.Events.Abstractions;

namespace Kafka.Common.Events.Update;

[KafkaEvent(EventKind.KEventUpdate)]
public sealed class UpdateEvent : IEvent
{
    public required Guid ResourceId { get; init; } = Guid.NewGuid();
    public required DateTime TimestampUtc { get; init; } = DateTime.UtcNow;

    [JsonIgnore] public Context Context { get; set; } = Context.Empty;

    public override string ToString()
    {
        return $"[{nameof(UpdateEvent)}] {JsonSerializer.Serialize(this)}";
    }
}