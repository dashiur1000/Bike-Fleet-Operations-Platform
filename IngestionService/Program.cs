using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IngestionService.Services;
using IngestionService.Kafka;

class Program
{
    static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddHttpClient();

                services.AddSingleton<KafkaProducerService>();

                services.AddHostedService<StationStatusPollingWorker>();
                services.AddHostedService<StationInformationPollingWorker>();
                services.AddHostedService<vehicleTypesPollingWorker>();
            })
            .Build();

        await host.RunAsync();
    }
}