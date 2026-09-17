using aspAPI.Data;
using aspAPI.Models;
using aspAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Data;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- MongoDB Settings & Services ---
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("station_status_history"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;

    if (string.IsNullOrWhiteSpace(options?.ConnectionString))
        throw new ArgumentException("Missing ConnectionString in 'station_status_history' configuration.");

    return new MongoClient(options.ConnectionString);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var options = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();

    if (string.IsNullOrWhiteSpace(options?.DatabaseName))
        throw new ArgumentException("Missing DatabaseName in 'station_status_history' configuration.");

    return client.GetDatabase(options.DatabaseName);
});

builder.Services.AddScoped<IMongoCollection<StationStatusDto>>(sp =>
{
    var options = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    var database = sp.GetRequiredService<IMongoDatabase>();

    var collectionName = string.IsNullOrWhiteSpace(options.CollectionName) ? "station_status_history" : options.CollectionName;
    return database.GetCollection<StationStatusDto>(collectionName);
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration["Redis:ConnectionString"] ?? builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    return ConnectionMultiplexer.Connect(configuration);
});


builder.Services.AddScoped<IStatusRepository, StatusRepository>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); builder.Services.AddDbContext<BikeDbContext>(options =>
    options.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(connectionString))
);
builder.Services.AddScoped<IDbConnection>(sp => new MySqlConnection(connectionString));


var app = builder.Build();

// Configure the HTTP request pipeline.
//app.UseGlobalExceptionMiddleware();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();