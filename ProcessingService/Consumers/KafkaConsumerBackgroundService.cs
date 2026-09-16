using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProcessingService.Handlers;

namespace ProcessingService.Consumers
{
    public class KafkaConsumerBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _bootstrapServers = "kafka:9092";
        private readonly string _groupId = "bike-processing-group";

        public KafkaConsumerBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };
            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            consumer.Subscribe(new[]
            {
                "bike.station-information",
                "bike.vehicle-types",
                "bike.station-status"
            });

            Console.WriteLine("[KafkaConsumer] Background Service is running and listening to topics...");

            await Task.Yield();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);

                    if (consumeResult?.Message == null) continue;

                    string topic = consumeResult.Topic;
                    string messageValue = consumeResult.Message.Value;

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        switch (topic)
                        {
                            case "bike.station-information":
                                var stationInfoHandler = scope.ServiceProvider.GetRequiredService<StationInformationHandler>();
                                await stationInfoHandler.HandleAsync(messageValue);
                                break;

                            case "bike.vehicle-types":
                                var vehicleTypesHandler = scope.ServiceProvider.GetRequiredService<VehicleTypesHandler>();
                                await vehicleTypesHandler.HandleAsync(messageValue);
                                break;

                            case "bike.station-status":
                                var stationStatusHandler = scope.ServiceProvider.GetRequiredService<StationStatusHandler>();
                                await stationStatusHandler.HandleAsync(messageValue);
                                break;

                            default:
                                Console.WriteLine($"[KafkaConsumer] Unknown topic received: {topic}");
                                break;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[KafkaConsumer Error] {ex.Message}");
                }
            }
            consumer.Close();
        }
    }
}