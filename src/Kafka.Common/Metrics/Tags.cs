namespace Kafka.Common.Metrics;

public static class Tags
{
    public const string MeterName = "Kafka.Metrics";
    
    private const string Base = "kafka";
    public static class Topic 
    {
        private const string TopicBase = $"{Base}.topic";
        
        public const string Name = $"{TopicBase}.name";
        public const string Partition = $"{TopicBase}.partition";
    }

    public static class Event
    {
        private const string EventBase = $"{Base}.event";
        public const string Kind = $"{EventBase}.kind";
    }

    public static class Exception
    {
        private const string ExceptionBase = $"{Base}.exception";
        public const string Type = $"{ExceptionBase}.type";
    }
}