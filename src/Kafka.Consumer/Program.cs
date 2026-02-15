using Kafka.Common;
using Kafka.Common.Events;
using Kafka.Common.Events.Abstractions;
using Kafka.Common.Serializer;
using Kafka.Consumer;
using Kafka.ServiceDefaults;
using Mediator;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMediator(options => { options.NotificationPublisherType = typeof(TaskWhenAllPublisher); });

// Source Generator automatically create implementation type for all matching generic types. e.g CreateEvent, DeleteEvent, GetEvent..
builder.Services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(KafkaEventHandlerErrorBehavior<,>));

builder.Services.AddOptionsWithValidateOnStart<KafkaOptions>()
    .Configure(opts =>
    {
        var brokenConnectionString = builder.Configuration.GetConnectionString("kafka-broker");

        if (!string.IsNullOrWhiteSpace(brokenConnectionString) && string.IsNullOrWhiteSpace(opts.BoostrapServer))
            opts.BoostrapServer = brokenConnectionString;
    })
    .BindConfiguration(KafkaOptions.SectionName)
    .ValidateDataAnnotations();

builder.Services.AddHostedService<KafkaBackgroundConsumer>();

/*
 * Producer needed in a consumer context for retry
 */
builder.AddKafkaProducer<Guid, IEvent>("kafka-broker",
    settings =>
    {
        settings.SetValueSerializer(new KafkaJsonSerializer());
        settings.SetKeySerializer(new KafkaGuidSerializer());
    });

var host = builder.Build();

await host.RunAsync();
await host.WaitForShutdownAsync();