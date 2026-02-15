using Confluent.Kafka;
using Kafka.Common.Events.Abstractions;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kafka.Common.Events;

public sealed class KafkaEventHandlerErrorBehavior<TEvent, TResponse>(
    ILogger<KafkaEventHandlerErrorBehavior<TEvent, TResponse>> logger,
    IProducer<Guid, IEvent> producer,
    IOptions<KafkaOptions> options)
    : MessageExceptionHandler<TEvent, TResponse> where TEvent : IEvent
{
    private readonly KafkaOptions _options = options.Value;

    protected override async ValueTask<ExceptionHandlingResult<TResponse>> Handle(TEvent @event, Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Error handling event of type {messageType}", @event.GetType().Name);

        var retryCount = @event.Context.Headers.FindRetryCount();

        var message = new Message<Guid, IEvent>
        {
            Value = @event,
            Key = @event.Context.Key,
            Headers = @event.Context.Headers
        };

        message.Headers.IncRetryCount();

        var topic = retryCount >= _options.MaxRetryCount ? _options.DlqTopic : _options.Topic;

        try
        {
            await producer.ProduceAsync(topic, message, cancellationToken);
            return Handled(default!);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to retry Event {Event}, RetryCount {RetryCount}, Topic {Topic}",
                @event.GetType().Name, retryCount, topic);
            return NotHandled;
        }
    }
}