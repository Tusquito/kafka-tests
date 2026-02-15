using Confluent.Kafka;
using Kafka.Common;
using Kafka.Common.Events;
using Kafka.Common.Events.Abstractions;
using Kafka.Common.Events.Null;
using Kafka.Common.Events.Unparsable;
using Kafka.Common.Serializer;
using Mediator;
using Microsoft.Extensions.Options;

namespace Kafka.Consumer;

public class KafkaBackgroundConsumer(
    IOptions<KafkaOptions> options,
    ILogger<KafkaBackgroundConsumer> logger,
    ISender sender)
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
        using var consumer = new ConsumerBuilder<Guid, IEvent>(config)
            .SetValueDeserializer(new KafkaJsonSerializer())
            .SetKeyDeserializer(new KafkaGuidSerializer())
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

        try
        {
            consumer.Subscribe(_options.Topic);

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                ConsumeResult<Guid, IEvent>? result;

                try
                {
                    result = consumer.Consume(stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    logger.LogWarning(ex, "Error occured while consuming");
                    continue;
                }

                if (result.Message?.Value is null or NullEvent)
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

                result.Message.Value.Context = new Context(result.Message.Headers, result.Message.Key);

                try
                {
                    // Can not revert sender publish, so we first commit, if it fails, we can skip so it will be retried in next loop
                    consumer.Commit(result);
                }
                catch (KafkaException e)
                {
                    logger.LogError(e, "Error occured while commiting event");
                    continue;
                }

                // No exception can be thrown by the publisher, ArgumentNullException is covered by previous null check,
                // InvalidMessageException is covered by TreatWarningsAsErrors at build time, AggregateException is covered by MessageExceptionHandler
                await sender.Send(result.Message.Value, stoppingToken);
            }
        }
        catch (Exception e) // Catch OperationCanceledException of consume or error from topic subscribe
        {
            logger.LogWarning(e, "Consumer stopped");
        }
        finally
        {
            consumer.Close();
        }
    }
}