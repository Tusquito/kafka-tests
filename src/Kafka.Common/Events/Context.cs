using Confluent.Kafka;

namespace Kafka.Common.Events;

public sealed record Context(Headers Headers, Guid Key)
{
    public static readonly Context Empty = new([], Guid.Empty);
}