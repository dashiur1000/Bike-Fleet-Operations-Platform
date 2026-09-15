using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IngestionService.Kafka
{
    public class KafkaProducerService : IDisposable
    {
        private readonly IProducer<string, string> _producer;
        public KafkaProducerService()
        {
            var bootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS") ?? "kafka:9092";

            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }
        public async Task ProduceAsync<T>(string topicName, string key, T data)
        {
            try
            {
                string jsonMessage = JsonSerializer.Serialize(data);
                var message = new Message<string, string>
                {
                    Key = key,
                    Value = jsonMessage
                };
                var deliveryResult = await _producer.ProduceAsync(topicName, message);
            }
            catch (ProduceException<string, string> ex)
            {
                Console.WriteLine(ex.Error.Reason);
            }
        }
        public void Dispose()
        {
            _producer?.Flush(TimeSpan.FromSeconds(5));
            _producer?.Dispose();
        }
    }
}
