using Confluent.Kafka;
using Kafka.Common;
using Kafka.Common.Events;
using Kafka.Common.Events.Abstractions;
using Kafka.Common.Json;
using Mediator;
using Microsoft.Extensions.Options;

namespace Kafka.Consumer;

public class KafkaBackgroundConsumer(
    IOptions<KafkaOptions> options,
    ILogger<KafkaBackgroundConsumer> logger,
    IMediator mediator)
    : BackgroundService
{
    /// <summary>
    ///     All types that have the <see cref="KafkaEventAttribute" /> attribute
    /// </summary>
    private readonly KafkaOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.BoostrapServer,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            GroupId = _options.GroupId
        };

        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_options.LoopIntervalMs));
        using var consumer = new ConsumerBuilder<string, IEvent>(config)
            .SetValueDeserializer(new JsonValueSerializer())
            .SetLogHandler((_, message) =>
            {
                var logLevel = message.Level.ToLogLevel();
                logger.LogInterceptedFromConsumerLogHandler(logLevel, message.Name, message.Facility, message.Message);
            })
            .SetErrorHandler((_, error) =>
            {
                logger.LogInterceptedFromConsumerErrorHandler(nameof(error.Code), error.Reason, (int)error.Code);
            })
            .SetOffsetsCommittedHandler((_, offsets) =>
            {
                foreach (var offset in offsets.Offsets)
                    logger.LogOffsetCommitted(offset.Topic, offset.Partition, offset.Offset);
            })
            .SetPartitionsAssignedHandler((_, partitions) =>
            {
                foreach (var partition in partitions) logger.LogPartitionAssigned(partition.Topic, partition.Partition);
            })
            .SetPartitionsLostHandler((_, partitions) =>
            {
                foreach (var partition in partitions)
                    logger.LogPartitionLost(partition.Topic, partition.Partition, partition.Offset);
            })
            .SetPartitionsRevokedHandler((_, partitions) =>
            {
                foreach (var partition in partitions)
                    logger.LogPartitionRevoked(partition.Topic, partition.Partition, partition.Offset);
            })
            .Build();

        consumer.Subscribe(_options.Topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    if (result.Message == null || result.Message.Value is NullEvent)
                    {
                        logger.LogEmptyMessageReceived(result.Partition, result.Offset);
                        continue;
                    }

                    if (result.Message.Value is UnparsableEvent unparsableEvent)
                    {
                        logger.LogUnparsableMessage(unparsableEvent.Data.Length, result.Message.Key, result.Partition,
                            result.Offset, result.Topic, unparsableEvent.Reason);
                        continue;
                    }

                    logger.LogEventReceived(result.Message.Key, result.Message.Value, result.Partition,
                        result.Offset);

                    await mediator.Publish(result.Message.Value, stoppingToken);

                    consumer.Commit(result);
                }
                catch (ConsumeException e)
                {
                    logger.LogWarning(e, "Error occured while consuming");
                }
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Consumer stopped");
        }
        finally
        {
            consumer.Close();
        }
    }
}