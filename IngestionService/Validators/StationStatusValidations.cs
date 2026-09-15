using IngestionService.DTOs;
using IngestionService.Kafka;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IngestionService.Validators
{
    public class StationStatusValidations
    {
        public StationStatusDto ValidateAndDeserialize(string StationStatusJson)
        {
            try
            {
                if (StationStatusJson == null) { return null; }
                var stationStatusDto = JsonSerializer.Deserialize<StationStatusDto>(StationStatusJson);
                if(stationStatusDto != null
                    && stationStatusDto.station_id != null
                    && stationStatusDto.num_vehicles_available >= 0
                    && stationStatusDto.num_docks_available >= 0)
                {
                    return stationStatusDto;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in StationStatus Worker: {ex.Message} - Inner: {ex.InnerException?.Message}");
            }
            return null;
        }
    }
}
