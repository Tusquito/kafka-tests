using Confluent.Kafka;
using Kafka.Common.Events.Abstractions;
using Kafka.Common.Metrics;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kafka.Common.Events;

public sealed class KafkaEventHandlerErrorBehavior<TEvent, TResponse>(
    ILogger<KafkaEventHandlerErrorBehavior<TEvent, TResponse>> logger,
    IProducer<Guid, IEvent> producer,
    IOptions<KafkaOptions> options,
    KafkaMetrics metrics)
    : MessageExceptionHandler<TEvent, TResponse> where TEvent : IEvent
{
    private readonly KafkaOptions _options = options.Value;

    protected override async ValueTask<ExceptionHandlingResult<TResponse>> Handle(TEvent @event, Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Error handling event of type {messageType}", @event.GetType().Name);

        var retryAttempts = @event.Context.Headers.FindRetryAttempts();

        var message = new Message<Guid, IEvent>
        {
            Value = @event,
            Key = @event.Context.Key,
            Headers = @event.Context.Headers
        };

        if (retryAttempts < _options.MaxRetryAttempts)
        {
            message.Headers.IncRetryAttempts();
        }
        
        var topic = retryAttempts >= _options.MaxRetryAttempts ? _options.DlqTopic : _options.Topic;

        try
        {
            await producer.ProduceAsync(topic, message, cancellationToken);
            metrics.RecordEventProduced(topic, message.Headers.FindEventKind());
            
            logger.LogInformation("Event {Event} has been published to {Topic} for retry. (Attempts {RetryAttempt}/{MaxRetryAttempts}).", message.Value, topic, retryAttempts, _options.MaxRetryAttempts);
            return Handled(default!);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to retry Event {Event}, RetryAttempts {RetryAttempts}, Topic {Topic}",
                @event.GetType().Name, retryAttempts, topic);
            return NotHandled;
        }
    }
}