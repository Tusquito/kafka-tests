using System.Text;
using Confluent.Kafka;
using Kafka.Common;
using Kafka.Common.Events;
using Kafka.Common.Events.Abstractions;
using Kafka.Common.Events.Create;
using Microsoft.Extensions.Options;

namespace Kafka.Producer;

public class KafkaBackgroundProducer(
    ILogger<KafkaBackgroundProducer> logger,
    IProducer<Guid, IEvent> producer,
    IOptions<KafkaOptions> options)
    : BackgroundService
{
    private readonly Dictionary<EventKind, Type> _events = KafkaEventExtensions.ScanEventTypes();
    private readonly KafkaOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_options.LoopIntervalMs));
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            var message = new Message<Guid, IEvent>();

            var rdmEvent = _events.PickRandomEvent();

            message.Headers =
            [
                new Header(KafkaHeader.EventKind.ToKey(), Encoding.UTF8.GetBytes(rdmEvent.Key.ToString())),
                new Header(KafkaHeader.RetryCount.ToKey(), [0])
            ];

            if (Activator.CreateInstance(rdmEvent.Value) is not IEvent evt)
            {
                logger.LogFailedToCreateEventInstance(rdmEvent.Value, rdmEvent.Key);
                continue;
            }

            message.Key = Guid.NewGuid();
            message.Value = evt;

            try
            {
                await producer.ProduceAsync(_options.Topic, message, stoppingToken);
                logger.LogEventEmitted(evt);
            }
            catch (ProduceException<Guid, IEvent> e)
            {
                logger.LogFailedToProduceEvent(evt, e);
            }
        }
    }
}