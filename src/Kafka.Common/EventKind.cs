namespace Kafka.Common;

public enum EventKind
{
    KEventUnknown,
    KEventGet,
    KEventUpdate,
    KEventCreate,
    KEventDelete
}