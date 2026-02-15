using Confluent.Kafka;
using Kafka.Common.Events.Abstractions;
using Kafka.Common.Events.Unparsable;
using Microsoft.Extensions.Logging;

namespace Kafka.Common;

public static partial class LoggerExtensions
{
    [LoggerMessage(LogLevel.Information, "New message handled: {Event}")]
    public static partial void LogEventHandled(this ILogger logger, IEvent @event);

    [LoggerMessage(LogLevel.Information, "New message emitted: {Event}")]
    public static partial void LogEventEmitted(this ILogger logger, IEvent @event);

    [LoggerMessage(LogLevel.Information,
        "New message received: [{Key}]: {Event} at Partition [{Partition}] and Offset [{Offset}] from Topic [{Topic}]")]
    public static partial void LogEventReceived(this ILogger logger, Guid key, IEvent @event, int partition,
        long offset, string topic);

    [LoggerMessage(LogLevel.Warning, "Failed to produce event: {Event}")]
    public static partial void LogFailedToProduceEvent(this ILogger logger, IEvent @event, Exception exception);

    [LoggerMessage(LogLevel.Warning, "Failed to create event of type [{EventType}] and Kind [{EventKind}]")]
    public static partial void LogFailedToCreateEventInstance(this ILogger logger, Type eventType, EventKind eventKind);

    [LoggerMessage(LogLevel.Warning,
        "An empty message was received at Offset [{Offset}] of Partition [{Partition}] from Topic [{Topic}]")]
    public static partial void LogEmptyMessageReceived(this ILogger logger, int partition, long offset, string topic);

    [LoggerMessage(LogLevel.Warning,
        "Failed to deserialize message ({Length} bytes) with Key [{Key}] at Offset [{Offset}] of Partition [{Partition}] from Topic [{Topic}] for Reason [{Reason}]")]
    public static partial void LogUnparsableMessage(this ILogger logger, long length, Guid key, int partition,
        long offset,
        string topic, UnparsableReason reason);

    [LoggerMessage("Log intercepted fron consumer log handler: [{Instance}][{Facility}] {Message}")]
    public static partial void LogInterceptedFromConsumerLogHandler(this ILogger logger, LogLevel level,
        string instance, string facility, string message);

    [LoggerMessage(LogLevel.Information,
        "Offset [{Offset}] has been commited to Partition [{Partition}] of Topic [{Topic}]")]
    public static partial void LogOffsetCommitted(this ILogger logger, string topic, int partition, long offset);

    [LoggerMessage(LogLevel.Information,
        "Partition [{Partition}] has been assigned to Topic [{Topic}]")]
    public static partial void LogPartitionAssigned(this ILogger logger, string topic, int partition);

    [LoggerMessage(LogLevel.Information,
        "Partition [{Partition}] with Offset [{Offset}] has been lost from Topic [{Topic}]")]
    public static partial void LogPartitionLost(this ILogger logger, string topic, int partition, long offset);

    [LoggerMessage(LogLevel.Information,
        "Partition [{Partition}] with Offset [{Offset}] has been revoked from Topic [{Topic}]")]
    public static partial void LogPartitionRevoked(this ILogger logger, string topic, int partition, long offset);

    [LoggerMessage(LogLevel.Information, "An error occured while consuming: [{CodeName}] {Reason} ({Code})")]
    public static partial void LogInterceptedFromConsumerErrorHandler(this ILogger logger, string codeName,
        string reason, int code);

    [LoggerMessage(LogLevel.Information, "An error occured while consuming a message.")]
    public static partial void LogConsumeFailed(this ILogger logger, Exception exception);

    [LoggerMessage(LogLevel.Warning,
        "An error occured while committing Event {Event} for key {Key} in Topic [{Topic}] at Partition [{Partition}] and Offset [{Offset}], Retrying... (Attempts {RetryAttempt}/{MaxRetryAttempts})")]
    public static partial void LogCommitFailed(this ILogger logger, Exception ex, IEvent @event, Guid key, string topic,
        int partition, long offset, int retryAttempt, int maxRetryAttempts);

    [LoggerMessage(LogLevel.Error,
        "Maximum retry attempts exceeded while committing Event {Event} for Key [{Key}] in Topic [{Topic}] at Partition [{Partition}] and Offset [{Offset}], exiting loop to avoid infinite loop.")]
    public static partial void LogCommitRetryAttemptsExceeded(this ILogger logger, IEvent @event, Guid key,
        string topic, int partition, long offset);
    
    [LoggerMessage(LogLevel.Warning, "An error occured in the consumer loop, stopping...")]
    public static partial void LogConsumerErrorOccured(this ILogger logger, Exception exception);


    public static LogLevel ToLogLevel(this SyslogLevel level)
    {
        return level switch
        {
            SyslogLevel.Emergency or SyslogLevel.Alert or SyslogLevel.Critical => LogLevel.Critical,
            SyslogLevel.Error => LogLevel.Error,
            SyslogLevel.Warning => LogLevel.Warning,
            SyslogLevel.Notice or SyslogLevel.Info => LogLevel.Information,
            SyslogLevel.Debug => LogLevel.Debug,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
    }
}