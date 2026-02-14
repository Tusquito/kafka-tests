using Confluent.Kafka;
using Kafka.Common.Events.Abstractions;

namespace Kafka.Common.Events;

public sealed record UnparsableEvent(
    SerializationContext Context,
    ReadOnlyMemory<byte> Data,
    UnparsableReason Reason = UnparsableReason.Unknown) : IEvent
{
    public static readonly UnparsableEvent Null = new(SerializationContext.Empty, ReadOnlyMemory<byte>.Empty);
}

public enum UnparsableReason
{
    Unknown,
    InvalidMessageComponentType,
    UnknownEventKind,
    NotDeserializableData
}