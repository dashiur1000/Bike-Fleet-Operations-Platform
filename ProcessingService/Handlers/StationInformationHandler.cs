using ProcessingService.Data;
using ProcessingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProcessingService.Handlers
{
    public class StationInformationHandler
    {
        private readonly SqlDbContext _db;
        public StationInformationHandler(SqlDbContext db)
        {
            _db = db;
        }
        public async Task HandleAsync(string stationInfoJson)
        {
            try
            {
                var incomingStation = JsonSerializer.Deserialize<StationInformationDto>(stationInfoJson);
                if(incomingStation == null || string.IsNullOrEmpty(incomingStation.station_id))
                {
                    return;
                }
                var existingStation = await _db.information
                    .FirstOrDefaultAsync(s => s.station_id == incomingStation.station_id);
                if(existingStation == null)
                {
                    _db.information.Add(incomingStation);
                }
                else
                {
                    existingStation.short_name = incomingStation.short_name;
                    existingStation.lat = incomingStation.lat;
                    existingStation.lon = incomingStation.lon;
                    existingStation.capacity = incomingStation.capacity;

                    _db.information.Update(existingStation);
                }
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling station information: {ex.Message}");
            }

        }
    }
}
