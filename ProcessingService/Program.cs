using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProcessingService.Consumers;
using ProcessingService.Data;
using ProcessingService.Handlers;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<SqlDbContext>(options =>
    options.UseMySql("Server=mysql;Database=BikeFleet;Uid=root;Pwd=yourpassword;",
        ServerVersion.AutoDetect("Server=mysql;Database=BikeFleet;Uid=root;Pwd=yourpassword;")));

builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("redis:6379"));

builder.Services.AddTransient<StackExchange.Redis.IDatabase>(sp =>
    sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

builder.Services.AddTransient<StationInformationHandler>();
builder.Services.AddTransient<VehicleTypesHandler>();
builder.Services.AddTransient<StationStatusHandler>();

builder.Services.AddHostedService<KafkaConsumerBackgroundService>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SqlDbContext>();
    db.Database.EnsureCreated();
}

host.Run();