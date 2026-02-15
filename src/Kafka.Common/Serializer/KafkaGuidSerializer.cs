using System.Text;
using Confluent.Kafka;

namespace Kafka.Common.Serializer;

public class KafkaGuidSerializer : IDeserializer<Guid>, ISerializer<Guid>
{
    public Guid Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return Guid.TryParse(data, out var guid) ? guid : Guid.Empty;
    }

    public byte[] Serialize(Guid data, SerializationContext context)
    {
        return Encoding.UTF8.GetBytes(data.ToString());
    }
}