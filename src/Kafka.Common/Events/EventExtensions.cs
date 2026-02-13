using System.Reflection;
using System.Text;
using Confluent.Kafka;

namespace Kafka.Common.Events;

public static class EventExtensions
{
    public const string EventKindHeaderKey = "event-kind";

    private static readonly Random Random = new(Guid.NewGuid().GetHashCode());

    public static Dictionary<EventKind, Type> ScanEventTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
                a.GetTypes().Where(t => t.GetCustomAttribute<KafkaEventAttribute>() != null))
            .ToDictionary(x => x.GetCustomAttribute<KafkaEventAttribute>()!.Kind, x => x);
    }

    public static EventKind FindEventKind(this Headers headers)
    {
        var kindBytes = headers.FirstOrDefault(x => x.Key == EventKindHeaderKey)?.GetValueBytes() ?? [];

        if (kindBytes.Length == 0) return EventKind.K_EVENT_UNKNOWN;

        var kindStr = Encoding.UTF8.GetString(kindBytes);

        if (string.IsNullOrWhiteSpace(kindStr)) return EventKind.K_EVENT_UNKNOWN;

        return Enum.TryParse(kindStr, out EventKind kind) ? kind : EventKind.K_EVENT_UNKNOWN;
    }

    extension(Dictionary<EventKind, Type> events)
    {
        public Type FindEventType(EventKind kind)
        {
            return events[kind];
        }

        public KeyValuePair<EventKind, Type> PickRandomEvent()
        {
            return events.ElementAt(Random.Next(0, events.Count - 1));
        }
    }
}