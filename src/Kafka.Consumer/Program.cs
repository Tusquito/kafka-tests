using Kafka.Common;
using Kafka.Consumer;
using Kafka.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMediator();

builder.Services.AddOptionsWithValidateOnStart<KafkaOptions>()
    .Configure(opts =>
    {
        var brokenConnectionString = builder.Configuration.GetConnectionString("kafka-broker");

        if (!string.IsNullOrWhiteSpace(brokenConnectionString) && string.IsNullOrWhiteSpace(opts.BoostrapServer))
        {
            opts.BoostrapServer =  brokenConnectionString;
        }
    })
    .BindConfiguration(KafkaOptions.SectionName)
    .ValidateDataAnnotations();

builder.Services.AddHostedService<KafkaBackgroundConsumer>();

var host = builder.Build();

await host.RunAsync();
await host.WaitForShutdownAsync();