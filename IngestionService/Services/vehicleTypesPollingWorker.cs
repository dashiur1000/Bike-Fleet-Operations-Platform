using IngestionService.Kafka;
using IngestionService.Validators;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IngestionService.Services
{
    internal class vehicleTypesPollingWorker : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly KafkaProducerService _kafkaProducerService;
        private readonly vehicleTypesValitations _vehicleTypesValitations;

        public vehicleTypesPollingWorker(IHttpClientFactory httpClientFactory, KafkaProducerService kafkaProducer)
        {
            _httpClientFactory = httpClientFactory;
            _kafkaProducerService = kafkaProducer;
            _vehicleTypesValitations = new vehicleTypesValitations();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var interval = TimeSpan.FromHours(1);
            using var timer = new PeriodicTimer(interval);

            try
            {
                await FetchAndPublishDataAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in initial fetch: {ex.Message}");
            }

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await FetchAndPublishDataAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR in Worker: {ex.Message} - Inner: {ex.InnerException?.Message}");
                }
            }
        }

        private async Task FetchAndPublishDataAsync(CancellationToken stoppingToken)
        {
            var client = _httpClientFactory.CreateClient();
            string url = "https://gbfs.lyft.com/gbfs/2.3/bkn/en/vehicle_types.json";

            HttpResponseMessage response = await client.GetAsync(url, stoppingToken);
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync(stoppingToken);
                var rootDto = _vehicleTypesValitations.ValidateAndDeserialize(jsonResponse);

                if (rootDto?.data?.vehicle_types != null)
                {
                    string topicName = "bike.vehicle-types";

                    foreach (var vehicleType in rootDto.data.vehicle_types)
                    {
                        string messageKey = vehicleType.vehicle_type_id;
                        await _kafkaProducerService.ProduceAsync(topicName, messageKey, vehicleType);
                    }
                }
                else
                {
                    Console.WriteLine("vehicle-types validation failed. Data skipped.");
                }
            }
        }
    }
}