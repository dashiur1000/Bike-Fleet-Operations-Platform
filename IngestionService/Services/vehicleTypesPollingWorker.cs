using IngestionService.Kafka;
using IngestionService.Validators;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    string url = "https://gbfs.lyft.com/gbfs/2.3/bkn/en/vehicle_types.json";
                    
                    HttpResponseMessage response = await client.GetAsync(url, stoppingToken);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync(stoppingToken);
                        var validatedDto = _vehicleTypesValitations.ValidateAndDeserialize(jsonResponse);
                        if(validatedDto != null)
                        {
                            string topicName = "bike.vehicle-types";
                            string messageKey = validatedDto.vehicle_type_id;

                            await _kafkaProducerService.ProduceAsync(topicName, messageKey, validatedDto);
                        }
                        else
                        {
                            Console.WriteLine("vehicle-types validation failed. Data skipped.");
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
