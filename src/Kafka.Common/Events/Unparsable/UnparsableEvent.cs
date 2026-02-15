using System.Text.Json.Serialization;
using Kafka.Common.Events.Abstractions;

namespace Kafka.Common.Events.Unparsable;

public sealed class UnparsableEvent : IEvent
{
    public static readonly UnparsableEvent Null = new();

    public ReadOnlyMemory<byte> Data { get; init; } = ReadOnlyMemory<byte>.Empty;
    public UnparsableReason Reason { get; init; } = UnparsableReason.Unknown;

    [JsonIgnore] public Context Context { get; set; } = Context.Empty;
}