using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;
using System.Text.Json;

namespace Post.Query.Infrastructure.Consumers;

public class EventConsumer : IEventConsumer
{
    private readonly ConsumerConfig consumerConfig;
    private readonly ILogger<EventConsumer> logger;
    private readonly IEventHandler eventHandler;

    public EventConsumer(ILogger<EventConsumer> logger, IOptions<ConsumerConfig> consumerConfig, IEventHandler eventHandler)
    {
        this.logger = logger;
        this.consumerConfig = consumerConfig.Value;
        this.eventHandler = eventHandler;
    }

    public void Consume(string topic)
    {
        try
        {
            using var consumer = new ConsumerBuilder<string, string>(consumerConfig)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(Deserializers.Utf8)
                .Build();

            consumer.Subscribe(topic);

            while (true)
            {
                var consumerResult = consumer.Consume();

                if (consumerResult?.Message == null)
                {
                    continue;
                }

                var jsonSerializerOptions = new JsonSerializerOptions() { Converters = { new EventJsonConverter() } };

                var @event = JsonSerializer.Deserialize<BaseEvent>(consumerResult.Message.Value, jsonSerializerOptions)!;

                var handlerMethod = eventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });

                if (handlerMethod == null)
                {
                    throw new InvalidOperationException($"Could not find event handler method for event {@event.GetType()}");
                }

                handlerMethod.Invoke(eventHandler, new object[] { @event });
                consumer.Commit(consumerResult);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while consuming the topic {topic}", topic);
            throw;
        }
    }
}

public class ConsumerHostedService : IHostedService
{
    private readonly ILogger<ConsumerHostedService> logger;
    private readonly IServiceProvider serviceProvider;

    public ConsumerHostedService(ILogger<ConsumerHostedService> logger, IServiceProvider serviceProvider)
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Event consumer service running");

        using (var scope = serviceProvider.CreateScope())
        {
            var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC")!;

            Task.Run(() => eventConsumer.Consume(topic), cancellationToken);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Event consumer service stopped");

        return Task.CompletedTask;
    }
}
