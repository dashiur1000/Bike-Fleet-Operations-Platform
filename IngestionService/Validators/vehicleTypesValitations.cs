using IngestionService.DTOs;
using System.Text.Json;

namespace IngestionService.Validators
{
    public class vehicleTypesValitations
    {
        public VehicleTypesDto ValidateAndDeserialize(string vehicleTypeJson)
        {
            try
            {
                if (string.IsNullOrEmpty(vehicleTypeJson))
                {
                    return null;
                }
                var vehicleTypeDto = JsonSerializer.Deserialize<VehicleTypesDto>(vehicleTypeJson);
                if (vehicleTypeDto != null
                    && !string.IsNullOrEmpty(vehicleTypeDto.vehicle_type_id))
                {
                    return vehicleTypeDto;
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
