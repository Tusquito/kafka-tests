using Kafka.Common;
using Kafka.Common.Events.Abstractions;
using Kafka.Common.Json;
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

builder.AddKafkaProducer<string, IEvent>("kafka-broker",
    settings => { settings.SetValueSerializer(new JsonValueSerializer()); });

var host = builder.Build();

await host.RunAsync();
await host.WaitForShutdownAsync();