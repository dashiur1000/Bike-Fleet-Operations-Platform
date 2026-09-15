using IngestionService.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IngestionService.Validators
{
    public class StationInformationValidations
    {
        public StationInformationDto ValidateAndDeserialize(string StationInformationJson)
        {
            try
            {
                if(StationInformationJson != null)
                {
                    var stationInformationDto = JsonSerializer.Deserialize<StationInformationDto>(StationInformationJson);
                    if(stationInformationDto != null
                        && stationInformationDto.station_id != null
                        && stationInformationDto.lat >= -90 && stationInformationDto.lat <= 90
                        && stationInformationDto.lon >= -180 && stationInformationDto.lon <= 180
                        && stationInformationDto.capacity >= 0)
                    {
                        return stationInformationDto;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
    }
}
