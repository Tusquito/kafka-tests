using Mediator;

namespace Kafka.Common.Events.Abstractions;

public interface IEvent : ICommand
{
    public Context Context { get; set; }
}