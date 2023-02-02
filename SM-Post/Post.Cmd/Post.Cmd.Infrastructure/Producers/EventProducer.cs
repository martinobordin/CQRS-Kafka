using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Producers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Post.Cmd.Infrastructure.Producers;

public class EventProducer : IEventProducer
{
    private readonly ProducerConfig producerConfig;
    private readonly ILogger<EventProducer> logger;

    public EventProducer(ILogger<EventProducer> logger, IOptions<ProducerConfig> producerConfig)
    {
        this.producerConfig = producerConfig.Value;
        this.logger = logger;
    }

    public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
    {
        try
        {
            using var producer = new ProducerBuilder<string, string>(producerConfig)
                       .SetKeySerializer(Serializers.Utf8)
                       .SetValueSerializer(Serializers.Utf8)
                       .Build();

            var eventMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(@event, @event.GetType())
            };

            var deliveryResult = await producer.ProduceAsync(topic, eventMessage);

            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            {
                throw new Exception($"Could not produce {@event.GetType().Name} message to topic {topic} due to following reason: {deliveryResult.Message}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while producing an event for the topic {topic}", topic);
            throw;
        }
    }
}
