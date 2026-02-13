namespace Kafka.Common.Events;

[AttributeUsage(AttributeTargets.Class)]
public sealed class KafkaEventAttribute(EventKind kind) : Attribute
{
    public EventKind Kind { get; } = kind;
}