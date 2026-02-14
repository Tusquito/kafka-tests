using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Kafka.Common.Events;
using Kafka.Common.Events.Abstractions;

namespace Kafka.Common.Json;

public class JsonValueSerializer : IDeserializer<IEvent>, ISerializer<IEvent>
{
    private readonly Dictionary<EventKind, Type> _events = EventExtensions.ScanEventTypes();

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters = { new JsonEventConverter() }
    };

    public IEvent Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull) return new NullEvent(context);

        if (context.Component != MessageComponentType.Value)
            return new UnparsableEvent(context, new ReadOnlyMemory<byte>(data.ToArray()),
                UnparsableReason.InvalidMessageComponentType);

        var eventKind = context.Headers.FindEventKind();

        if (eventKind is EventKind.KEventUnknown)
            return new UnparsableEvent(context, new ReadOnlyMemory<byte>(data.ToArray()),
                UnparsableReason.UnknownEventKind);

        var eventType = _events.FindEventType(eventKind);

        var str = Encoding.UTF8.GetString(data);

        if (JsonSerializer.Deserialize(str, eventType, _serializerOptions) is not IEvent deserialized)
            return new UnparsableEvent(context, new ReadOnlyMemory<byte>(data.ToArray()),
                UnparsableReason.NotDeserializableData);

        return deserialized;
    }

    public byte[] Serialize(IEvent data, SerializationContext context)
    {
        return data is NullEvent or UnparsableEvent
            ? []
            : Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, _serializerOptions));
    }
}