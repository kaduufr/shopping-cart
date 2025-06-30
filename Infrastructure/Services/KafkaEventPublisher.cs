
namespace Infrastructure.Services;

public class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<Null, string> _producer;

    public KafkaEventPublisher(IProducer<Null, string> producer)
    {
        _producer = producer;
    }

    public async Task PublishAsync<T>(string topic, T message)
    {
        var serializedMessage = JsonSerializer.Serialize(message);
        await _producer.ProduceAsync(topic, new Message<Null, string> { Value = serializedMessage });
    }
}