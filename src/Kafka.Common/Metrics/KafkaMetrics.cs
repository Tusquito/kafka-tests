using System.Diagnostics.Metrics;
using System.Reflection;

namespace Kafka.Common.Metrics;

public sealed class KafkaMetrics
{
    private readonly Counter<long> _eventConsumedCounter;
    private readonly Counter<long> _eventProducedCounter;
    private readonly Counter<long> _eventConsumedExceptionsCounter;
    private readonly Counter<long> _eventProducedExceptionsCounter;

    public KafkaMetrics(IMeterFactory factory)
    {
        var metter = factory.Create(Tags.MeterName, Assembly.GetExecutingAssembly().GetName().Version?.ToString());
        _eventConsumedCounter = metter.CreateCounter<long>("kafka.event.consumed", "{events}",
            "Kafka events consumed by the consumer loop.", new List<KeyValuePair<string, object?>>
            {
                new(Tags.Topic.Name, null),
                new(Tags.Topic.Partition, null),
                new(Tags.Event.Kind, null)
            });

        _eventProducedCounter = metter.CreateCounter<long>("kafka.event.produced", "{events}",
            "Kafka events consumed by the consumer loop.", new List<KeyValuePair<string, object?>>
            {
                new(Tags.Topic.Name, null),
                new(Tags.Event.Kind, null)
            });

        _eventConsumedExceptionsCounter = metter.CreateCounter<long>("kafka.event.consumed.exceptions", "{exceptions}",
            "Kafka errors occured while consuming events", new List<KeyValuePair<string, object?>>
            {
                new(Tags.Topic.Name, null),
                new(Tags.Topic.Partition, null),
                new(Tags.Event.Kind, null),
                new(Tags.Exception.Type, null)
            });

        _eventProducedExceptionsCounter = metter.CreateCounter<long>("kafka.event.produced.exceptions", "{exceptions}",
            "Kafka errors occured while producing events", new List<KeyValuePair<string, object?>>
            {
                new(Tags.Topic.Name, null),
                new(Tags.Event.Kind, null),
                new(Tags.Exception.Type, null)
            });
    }

    public void RecordEventConsumed(string? topic, int? partition, EventKind? kind)
    {
        _eventConsumedCounter.Add(1,
            new KeyValuePair<string, object?>(Tags.Topic.Name, topic),
            new KeyValuePair<string, object?>(Tags.Event.Kind, kind.ToString()),
            new KeyValuePair<string, object?>(Tags.Topic.Partition, partition)
        );
    }

    public void RecordEventProduced(string? topic, EventKind? kind)
    {
        _eventProducedCounter.Add(1,
            new KeyValuePair<string, object?>(Tags.Topic.Name, topic),
            new KeyValuePair<string, object?>(Tags.Event.Kind, kind.ToString()));
    }

    public void RecordEventConsumedException(string? topic, int? partition, EventKind? kind, Type? exceptionType)
    {
        _eventConsumedExceptionsCounter.Add(1, new KeyValuePair<string, object?>(Tags.Topic.Name, topic),
            new KeyValuePair<string, object?>(Tags.Event.Kind, kind.ToString()),
            new KeyValuePair<string, object?>(Tags.Topic.Partition, partition),
            new KeyValuePair<string, object?>(Tags.Exception.Type, exceptionType?.Name));
    }

    public void RecordEventProducedException(string? topic, EventKind? kind, Type? exceptionType)
    {
        _eventConsumedExceptionsCounter.Add(1, [
            new KeyValuePair<string, object?>(Tags.Topic.Name, topic),
            new KeyValuePair<string, object?>(Tags.Event.Kind, kind.ToString()),
            new KeyValuePair<string, object?>(Tags.Exception.Type, exceptionType?.Name)
        ]);
    }
}