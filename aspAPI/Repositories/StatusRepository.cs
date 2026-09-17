using aspAPI.Data;
using aspAPI.DTOs;
using aspAPI.Models;
using Dapper;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MySqlConnector;
using StackExchange.Redis;
using System.Data;
using System.Text.Json;

namespace aspAPI.Repositories
{
    public class StatusRepository : IStatusRepository
    {
        public readonly IMongoCollection<StationStatusDto> _mongoCollection;
        private readonly StackExchange.Redis.IDatabase _redisDb;
        private readonly IDbConnection _mysqlConnection;
        private readonly BikeDbContext _context;
        public StatusRepository(IMongoCollection<StationStatusDto> mongoCollection, IConnectionMultiplexer redisConnection, IDbConnection dbConnection, BikeDbContext context)
        {
            _mongoCollection = mongoCollection;
            _redisDb = redisConnection.GetDatabase();
            _mysqlConnection = dbConnection;
            _context = context;
        }

        public async Task<StationStatusDto?> GetCurrentStationStatusAsync(string id)
        {
            string redisKey = $"station_status:{id}";
            string cachedJson = await _redisDb.StringGetAsync(redisKey);

            if (string.IsNullOrEmpty(cachedJson))
            {
                return null;
            }

            return JsonSerializer.Deserialize<StationStatusDto>(cachedJson);
        }
        public async Task<StationsDto?> GetById(string id)
        {
            var mongoCursor = await _mongoCollection.FindAsync(x => x.station_id == id);
            var mongoResults = await mongoCursor.FirstOrDefaultAsync();
            if (mongoResults == null)
            {
                return null;
            }
            var mysqlData = await _context.StationInformation.FirstOrDefaultAsync(a => a.station_id == id);
            var result = new StationsDto
            {
                station_id = id,
                short_name = mysqlData.short_name,
                num_docks_available = mongoResults.num_docks_available,
                capacity = mysqlData.capacity,
                is_renting = mongoResults.is_renting,
                is_returning = mongoResults.is_returning,
                last_reported = mongoResults.last_reported,
                lat = mysqlData.lat,
                lon = mysqlData.lon,
                num_vehicles_available = mongoResults.num_vehicles_available
            };
            return result;
        }
        public async Task<List<StationsDto>> GetByFilter(int? minAvailableBikes, int? isRenting, int? isReturning)
        {
            var filterBuilder = Builders<StationStatusDto>.Filter;
            var filter = filterBuilder.Empty;

            if(minAvailableBikes.HasValue)
            {
                filter &= filterBuilder.Gte(a => a.num_vehicles_available, minAvailableBikes);
            }
            if(isRenting.HasValue)
            {
                filter &= filterBuilder.Eq(a => a.is_renting, isRenting.Value);
            }
            if(isReturning != null)
            {
                filter &= filterBuilder.Eq(a => a.is_returning, isReturning.Value);
            }
            var mongoResults = await _mongoCollection.Find(filter).ToListAsync();
            if(!mongoResults.Any())
            {
                return new List<StationsDto>();
            }
            var stationIds = mongoResults.Select(s => s.station_id).Distinct().ToList();
            var sql = "SELECT station_id, short_name, lon, lat, capacity FROM information WHERE station_id IN @Ids";
            
            var mysqlData = await _mysqlConnection.QueryAsync<StationsDto>(sql, new { Ids = stationIds });
            var mysqlDict = mysqlData.ToDictionary(s => s.station_id);
            var resultList = new List<StationsDto>();
            foreach (var mongoItem in mongoResults)
            {
                var dto = new StationsDto
                {
                    station_id = mongoItem.station_id,
                    num_docks_available = mongoItem.num_docks_available,
                    num_vehicles_available = mongoItem.num_vehicles_available,
                    is_renting = mongoItem.is_renting,
                    is_returning = mongoItem.is_returning,
                    last_reported = mongoItem.last_reported
                };
                if(mysqlDict.TryGetValue(mongoItem.station_id, out var sqlInfo))
                {
                    dto.short_name = sqlInfo.short_name;
                    dto.lon = sqlInfo.lon;
                    dto.lat = sqlInfo.lat;
                    dto.capacity = sqlInfo.capacity;
                }
                resultList.Add(dto);
            }
            return resultList;
        }
        public async Task<List<HistoryDto>> GetStationHistory(DateTime? from, DateTime? to, int? limit)
        {
            var filterBuilder = Builders<StationStatusDto>.Filter;
            var filter = filterBuilder.Empty;

            if (from.HasValue)
            {
                filter &= filterBuilder.Gte(a => DateTimeOffset.FromUnixTimeSeconds(a.last_reported).UtcDateTime, from);
            }
            if (to.HasValue)
            {
                filter &= filterBuilder.Lte(a => DateTimeOffset.FromUnixTimeSeconds(a.last_reported).UtcDateTime, to);
            }
            var query = _mongoCollection.Find(filter);
            if(limit.HasValue)
            {
                query = query.Limit(limit.Value);
            }
            var mongoResults = await query.ToListAsync();
            List<HistoryDto> resultList = new List<HistoryDto>();
            foreach (var item in mongoResults)
            {
                string redisKey = $"station_status:{item.station_id}";
                string cachedJson = await _redisDb.StringGetAsync(redisKey);
                var cachedStatus = JsonSerializer.Deserialize<StationStatusDto>(cachedJson);
                if (cachedJson != null)
                {
                    var dto = new HistoryDto
                    {
                        timestamp = DateTimeOffset.FromUnixTimeSeconds(cachedStatus.last_reported).UtcDateTime,
                        availableBikes = item.num_vehicles_available,
                        availableDocks = item.num_docks_available
                    };
                    resultList.Add(dto);
                }
            }
            return resultList;
        }
    }
}