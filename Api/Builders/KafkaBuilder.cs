using Confluent.Kafka;
using Domain.Interfaces.Services;
using Infrastructure.BackgroundServices;
using Infrastructure.Services;

namespace Api.Builders;

public static class KafkaBuilder
{
    public static WebApplicationBuilder UseKafka(this WebApplicationBuilder builder)
    {
        // Kafka Producer
        var producerConfig = new ProducerConfig { BootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers") ?? "localhost:9092" };
        builder.Services.AddSingleton<IProducer<Null, string>>(new ProducerBuilder<Null, string>(producerConfig).Build());
        builder.Services.AddSingleton<IEventPublisher, KafkaEventPublisher>();

        // Kafka Consumer
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers") ?? "localhost:9092",
            GroupId = "cart-events",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        builder.Services.AddSingleton(new ConsumerBuilder<Ignore, string>(consumerConfig).Build());
        builder.Services.AddHostedService<CartEventsConsumer>();

        return builder;
    }
}