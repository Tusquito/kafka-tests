using Kafka.Common.Events.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kafka.Common;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "New message handled: {event}")]
    public static partial void LogEventHandled(this ILogger logger, IEvent @event);
    
    [LoggerMessage(Level = LogLevel.Information, Message = "New message emitted: {event}")]
    public static partial void LogEventEmitted(this ILogger logger, IEvent @event);
    
    [LoggerMessage(Level = LogLevel.Information, Message = "New message received: [{key}]: {message}")]
    public static partial void LogMessageReceived(this ILogger logger, string key, string message);
    
    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to produce event: {event}")]
    public static partial void LogFailedToProduceEvent(this ILogger logger, IEvent @event, Exception exception);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to invoke event for message: [{key}]: {message}")]
    public static partial void LogFailedToInvokeEvent(this ILogger logger, string key, string message);
    
    [LoggerMessage(Level = LogLevel.Warning, Message = "An empty message was received at [{partition}] {offset} ")]
    public static partial void LogEmptyMessageReceived(this ILogger logger, int partition,  long offset);
    
    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to deserialize message to even type `{eventTypeName}`: [{key}]: {message}")]
    public static partial void LogFailedToDeserializeMessage(this ILogger logger, string key, string message, string eventTypeName);
}