using System.ComponentModel.DataAnnotations;

namespace Kafka.Common;

public sealed class KafkaOptions
{
    public const string SectionName = "Kafka";

    [Required] public required string BoostrapServer { get; set; }

    [Required] public required string GroupId { get; init; }

    [Required] public required string Topic { get; init; }

    [Required] public required string DlqTopic { get; init; }

    [Range(0, long.MaxValue)] public required long LoopIntervalMs { get; init; } = 100;
}