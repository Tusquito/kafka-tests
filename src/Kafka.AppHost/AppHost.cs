var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddKafka("kafka-broker")
    .WithKafkaUI()
    .WithDataVolume(isReadOnly: false)
    .WithEnvironment("KAFKA_AUTO_CREATE_TOPICS_ENABLE", "true");

var producer = builder.AddProject<Projects.Kafka_Producer>("kafka-producer")
    .WaitFor(kafka)
    .WithReference(kafka);

builder.AddProject<Projects.Kafka_Consumer>("kafka-consumer")
    .WaitFor(producer)
    .WaitFor(kafka)
    .WithReference(kafka);

builder.Build().Run();