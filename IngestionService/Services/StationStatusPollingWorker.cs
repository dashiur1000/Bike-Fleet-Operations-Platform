using IngestionService.Kafka;
using IngestionService.Validators;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IngestionService.Services
{
    internal class StationStatusPollingWorker : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly KafkaProducerService _kafkaProducerService;
        private readonly StationStatusValidations _stationStatusValidations;

        public StationStatusPollingWorker(IHttpClientFactory httpClientFactory, KafkaProducerService kafkaProducer)
        {
            _httpClientFactory = httpClientFactory;
            _kafkaProducerService = kafkaProducer;
            _stationStatusValidations = new StationStatusValidations();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    string url = "https://gbfs.lyft.com/gbfs/2.3/bkn/en/station_status.json";
                    
                    HttpResponseMessage response = await client.GetAsync(url, stoppingToken);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync(stoppingToken);
                        var validatedDto = _stationStatusValidations.ValidateAndDeserialize(jsonResponse);
                        if(validatedDto != null)
                        {
                            string topicName = "bike.station-status";
                            string messageKey = validatedDto.station_id;

                            await _kafkaProducerService.ProduceAsync(topicName, messageKey, validatedDto);
                        }
                        else
                        {
                            Console.WriteLine("station-status validation failed. Data skipped.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR!");
                }
            }
        }
    }
}
