using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Kafka.Common.Events;
using Kafka.Common.Events.Abstractions;
using Kafka.Common.Events.Null;
using Kafka.Common.Events.Unparsable;
using Kafka.Common.Json;

namespace Kafka.Common.Serializer;

public sealed class KafkaJsonSerializer : IDeserializer<IEvent>, ISerializer<IEvent>
{
    private readonly Dictionary<EventKind, Type> _events = KafkaEventExtensions.ScanEventTypes();

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters = { new JsonEventConverter() }
    };

    public IEvent Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull || data.IsEmpty) return NullEvent.Null;

        var eventKind = context.Headers.FindEventKind();

        if (eventKind is EventKind.KEventUnknown)
            return new UnparsableEvent
            {
                Data = new ReadOnlyMemory<byte>(data.ToArray()),
                Reason = UnparsableReason.UnknownEventKind
            };

        var eventType = _events.FindEventType(eventKind);

        var str = Encoding.UTF8.GetString(data);

        if (JsonSerializer.Deserialize(str, eventType, _serializerOptions) is not IEvent deserialized)
            return new UnparsableEvent
            {
                Data = new ReadOnlyMemory<byte>(data.ToArray()),
                Reason = UnparsableReason.NotDeserializableData
            };


        return deserialized;
    }

    public byte[] Serialize(IEvent data, SerializationContext context)
    {
        return data is NullEvent or UnparsableEvent
            ? []
            : Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, _serializerOptions));
    }
}