using System.Text.Json.Serialization;
using Kafka.Common.Events.Abstractions;

namespace Kafka.Common.Events.Null;

public sealed class NullEvent : IEvent
{
    public static readonly NullEvent Null = new();

    [JsonIgnore] public Context Context { get; set; } = Context.Empty;
}