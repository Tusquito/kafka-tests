using Confluent.Kafka;
using Kafka.Common.Events;
using Kafka.Common.Events.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kafka.Common;

public static partial class LoggerExtensions
{
    [LoggerMessage(LogLevel.Information, "New message handled: {Event}")]
    public static partial void LogEventHandled(this ILogger logger, IEvent @event);

    [LoggerMessage(LogLevel.Information, "New message emitted: {Event}")]
    public static partial void LogEventEmitted(this ILogger logger, IEvent @event);

    [LoggerMessage(LogLevel.Information,
        "New message received: [{Key}]: {Event} at Partition [{Partition}] and Offset [{Offset}]")]
    public static partial void LogEventReceived(this ILogger logger, string key, IEvent @event, int partition,
        long offset);

    [LoggerMessage(LogLevel.Warning, "Failed to produce event: {Event}")]
    public static partial void LogFailedToProduceEvent(this ILogger logger, IEvent @event, Exception exception);

    [LoggerMessage(LogLevel.Warning, "Failed to create event of type [{EventType}] and Kind [{EventKind}]")]
    public static partial void LogFailedToCreateEventInstance(this ILogger logger, Type eventType, EventKind eventKind);

    [LoggerMessage(LogLevel.Warning, "An empty message was received at Offset [{Offset}] of Partition [{Partition}]")]
    public static partial void LogEmptyMessageReceived(this ILogger logger, int partition, long offset);

    [LoggerMessage(LogLevel.Warning,
        "Failed to deserialize message ({Length} bytes) with Key [{Key}] at Offset [{Offset}] of Partition [{Partition}] from Topic [{Topic}] for Reason [{Reason}]")]
    public static partial void LogUnparsableMessage(this ILogger logger, long length, string key, int partition,
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