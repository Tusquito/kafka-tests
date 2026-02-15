using System.Reflection;
using System.Text;
using Confluent.Kafka;

namespace Kafka.Common.Events;

public static class KafkaEventExtensions
{
    private static readonly Random Random = new(Guid.NewGuid().GetHashCode());

    public static Dictionary<EventKind, Type> ScanEventTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
                a.GetTypes().Where(t => t.GetCustomAttribute<KafkaEventAttribute>() != null))
            .ToDictionary(x => x.GetCustomAttribute<KafkaEventAttribute>()!.Kind, x => x);
    }

    extension(Headers headers)
    {
        public EventKind FindEventKind()
        {
            if (!headers.TryGetLastBytes(KafkaHeader.EventKind.ToKey(), out var bytes) || bytes.Length == 0)
                return EventKind.KEventUnknown;

            var kindStr = Encoding.UTF8.GetString(bytes);

            if (string.IsNullOrWhiteSpace(kindStr)) return EventKind.KEventUnknown;

            return Enum.TryParse(kindStr, out EventKind kind) ? kind : EventKind.KEventUnknown;
        }

        public byte FindRetryCount()
        {
            if (!headers.TryGetLastBytes(KafkaHeader.RetryCount.ToKey(), out var bytes) || bytes.Length == 0)
                return 0;

            return bytes.FirstOrDefault();
        }

        public byte IncRetryCount()
        {
            var key = KafkaHeader.RetryCount.ToKey();
            var nextCount = (byte)(headers.FindRetryCount() + 1);
            headers.Remove(key);
            headers.Add(key, [nextCount]);
            return nextCount;
        }
    }

    extension(KafkaHeader header)
    {
        public string ToKey()
        {
            return header.ToString().ToLowerInvariant();
        }
    }

    extension(Dictionary<EventKind, Type> events)
    {
        public Type FindEventType(EventKind kind)
        {
            return events[kind];
        }

        public KeyValuePair<EventKind, Type> PickRandomEvent()
        {
            return events.ElementAt(Random.Next(0, events.Count));
        }
    }
}