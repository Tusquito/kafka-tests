using System.Text.Json;
using System.Text.Json.Serialization;
using Kafka.Common.Events;
using Kafka.Common.Events.Abstractions;

namespace Kafka.Common.Json;

public sealed class JsonEventConverter : JsonConverter<IEvent>
{
    public override IEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return NullEvent.Null;

        if (reader.TokenType != JsonTokenType.String) return UnparsableEvent.Null;

        return JsonSerializer.Deserialize<IEvent>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, IEvent value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case null or NullEvent:
                writer.WriteNullValue();
                break;
            default:
                // Needed to take implementation type instead of interface
                var type = value.GetType();
                JsonSerializer.Serialize(writer, value, type, options);
                break;
        }
    }
}