using Microsoft.EntityFrameworkCore.Storage;
using ProcessingService.Data;
using System;
using System.Collections.Generic;
using System.Text.Json;
using ProcessingService.Data;
using ProcessingService.Models;
using StackExchange.Redis;
using MongoDB.Driver;

namespace ProcessingService.Handlers
{
    public class StationStatusHandler
    {
        private readonly MongoDbContext _mongoDbContext;
        private readonly StackExchange.Redis.IDatabase _redisDb;

        public StationStatusHandler(MongoDbContext mongoDbContext, StackExchange.Redis.IDatabase redisDb)
        {
            _mongoDbContext = mongoDbContext;
            _redisDb = redisDb;
        }
        public async Task HandleAsync(string stationStatusJson)
        {
            try
            {
                var newStatus = JsonSerializer.Deserialize<StationStatusDto>(stationStatusJson);
                if(newStatus == null || string.IsNullOrEmpty(newStatus.station_id))
                {
                    return;
                }
                string redisKey = $"station_status:{newStatus.station_id}";
                string lastStateJson = await _redisDb.StringGetAsync(redisKey);

                if(!string.IsNullOrEmpty(lastStateJson))
                {
                    var lastStatus = JsonSerializer.Deserialize<StationStatusDto>(lastStateJson);

                    if(lastStatus != null
                        && lastStatus.num_vehicles_available == newStatus.num_vehicles_available
                        && lastStatus.num_docks_available == lastStatus.num_docks_available)
                    {
                        return;
                    }
                }
                await _mongoDbContext.StationStatusHistory.InsertOneAsync(newStatus);
                await _redisDb.StringSetAsync(redisKey, stationStatusJson);
                Console.WriteLine($"[Change Detected & Saved] Station ID: {newStatus.station_id} updated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling station status: {ex.Message}");
            }
        }
    }
}
