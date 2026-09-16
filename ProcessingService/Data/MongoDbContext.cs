using MongoDB.Driver;
using ProcessingService.Models;

namespace ProcessingService.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext()
        {
            var connectionString = "mongodb://mongo:27017";

            var mongoUrl = MongoUrl.Create(connectionString);
            var client = new MongoClient(mongoUrl);

            _database = client.GetDatabase("BikeFleetDB");
        }

        public IMongoCollection<StationStatusDto> StationStatusHistory =>
            _database.GetCollection<StationStatusDto>("station_status_history");
    }
}