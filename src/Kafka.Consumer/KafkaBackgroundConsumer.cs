using System.Text.Json;
using Confluent.Kafka;
using Kafka.Common;
using Kafka.Common.Events;
using Mediator;
using Microsoft.Extensions.Options;

namespace Kafka.Consumer;

public class KafkaBackgroundConsumer : BackgroundService
{
    /// <summary>
    ///     All types that have the <see cref="KafkaEventAttribute" /> attribute
    /// </summary>
    private readonly Dictionary<EventKind, Type> _events = EventExtensions.ScanEventTypes();

    private readonly ILogger<KafkaBackgroundConsumer> _logger;
    private readonly IMediator _mediator;
    private readonly KafkaOptions _options;

    public KafkaBackgroundConsumer(IOptions<KafkaOptions> options, ILogger<KafkaBackgroundConsumer> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
        _options = options.Value;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.BoostrapServer,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            GroupId = _options.GroupId
        };

        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_options.LoopIntervalMs));
        using var consumer = new ConsumerBuilder<string, string>(config)
            .Build();

        consumer.Subscribe(_options.Topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    if (result.Message == null)
                    {
                        _logger.LogEmptyMessageReceived(result.Partition, result.Offset);
                        continue;
                    }

                    _logger.LogMessageReceived(result.Message.Key, result.Message.Value);

                    var eventKind = result.Message.Headers.FindEventKind();

                    var eventType = _events.FindEventType(eventKind);

                    var deserialized = JsonSerializer.Deserialize(result.Message.Value, eventType);

                    if (deserialized == null)
                    {
                        _logger.LogFailedToDeserializeMessage(result.Message.Key, result.Message.Value, eventType.Name);
                        continue;
                    }

                    await _mediator.Publish(deserialized, stoppingToken);
                }
                catch (ConsumeException e)
                {
                    _logger.LogWarning(e, "Error occured while consuming");
                }
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Consumer stopped");
        }
        finally
        {
            consumer.Close();
        }
    }
}