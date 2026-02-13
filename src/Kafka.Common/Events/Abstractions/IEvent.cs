using Mediator;

namespace Kafka.Common.Events.Abstractions;

public interface IEvent : INotification
{
    public Guid EventId { get; init; }
    public Guid ResourceId { get; init; }
    public DateTime TimestampUtc { get; init; }
}