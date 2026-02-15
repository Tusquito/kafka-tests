using Kafka.Common;
using Kafka.Common.Events.Abstractions;
using Kafka.Common.Serializer;
using Kafka.Producer;
using Kafka.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<KafkaBackgroundProducer>();

builder.Services.AddOptionsWithValidateOnStart<KafkaOptions>()
    .Configure(opts =>
    {
        var brokenConnectionString = builder.Configuration.GetConnectionString("kafka-broker");

        if (!string.IsNullOrWhiteSpace(brokenConnectionString) && string.IsNullOrWhiteSpace(opts.BoostrapServer))
            opts.BoostrapServer = brokenConnectionString;
    })
    .BindConfiguration(KafkaOptions.SectionName)
    .ValidateDataAnnotations();

builder.AddKafkaProducer<Guid, IEvent>("kafka-broker",
    settings =>
    {
        settings.SetValueSerializer(new KafkaJsonSerializer());
        settings.SetKeySerializer(new KafkaGuidSerializer());
    });

var host = builder.Build();

await host.RunAsync();
await host.WaitForShutdownAsync();