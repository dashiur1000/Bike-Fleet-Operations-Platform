using IngestionService.DTOs;
using IngestionService.Kafka;
using IngestionService.Validators;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace IngestionService.Services
{
    public class StationInformationPollingWorker : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly KafkaProducerService _kafkaProducerService;
        private readonly StationInformationValidations _informationValidations;

        public StationInformationPollingWorker(IHttpClientFactory httpClientFactory, KafkaProducerService kafkaProducer)
        {
            _httpClientFactory = httpClientFactory;
            _kafkaProducerService = kafkaProducer;
            _informationValidations = new StationInformationValidations();
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
            string url = "https://gbfs.lyft.com/gbfs/2.3/bkn/en/station_information.json";

            HttpResponseMessage response = await client.GetAsync(url, stoppingToken);
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync(stoppingToken);
                var rootDto = JsonSerializer.Deserialize<StationInformationRootDto>(jsonResponse);

                if (rootDto?.data?.stations != null)
                {
                    string topicName = "bike.station-information";

                    foreach (var station in rootDto.data.stations)
                    {
                        string stationJson = JsonSerializer.Serialize(station);
                        var validatedDto = _informationValidations.ValidateAndDeserialize(stationJson);

                        if (validatedDto != null)
                        {
                            string messageKey = validatedDto.station_id;
                            await _kafkaProducerService.ProduceAsync(topicName, messageKey, validatedDto);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("station_information validation failed. Data skipped.");
                }
            }
        }
    }
}