using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Kafka.Common;
using Kafka.Common.Events;
using Kafka.Common.Events.Abstractions;
using Kafka.Consumer;
using Microsoft.Extensions.Options;

namespace Kafka.Producer;

public class KafkaBackgroundProducer(ILogger<KafkaBackgroundProducer> logger, IProducer<string, string> producer, IOptions<KafkaOptions> options)
    : BackgroundService
{
    private readonly KafkaOptions _options = options.Value;

    private readonly Dictionary<EventKind, Type> _events = EventExtensions.ScanEvents();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_options.LoopIntervalMs));
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            var message = new Message<string, string>();

            var rdmEvent = _events.PickRandomEvent();

            message.Headers =
                [new Header(EventExtensions.EventKindHeaderKey, Encoding.UTF8.GetBytes(rdmEvent.Key.ToString()))];

            if (Activator.CreateInstance(rdmEvent.Value) is not IEvent evt)
            {
                logger.LogFailedToInvokeEvent(message.Key, message.Value);
                continue;
            }

            message.Key = Guid.NewGuid().ToString();
            message.Value = JsonSerializer.Serialize(evt);

            try
            {
                await producer.ProduceAsync(_options.Topic, message, stoppingToken);
                logger.LogEventEmitted(evt);
            }
            catch(ProduceException<string, string> e)
            {
                logger.LogFailedToProduceEvent(evt, e);
            }
        }
    }
}