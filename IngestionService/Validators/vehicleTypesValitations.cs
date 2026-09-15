using IngestionService.DTOs;
using System;
using System.Text.Json;

namespace IngestionService.Validators
{
    public class vehicleTypesValitations
    {
        public VehicleTypesRootDto ValidateAndDeserialize(string vehicleTypeJson)
        {
            try
            {
                if (string.IsNullOrEmpty(vehicleTypeJson))
                {
                    return null;
                }

                var rootDto = JsonSerializer.Deserialize<VehicleTypesRootDto>(vehicleTypeJson);

                if (rootDto?.data?.vehicle_types != null)
                {
                    return rootDto;
                }
                else
                {
                    Console.WriteLine("Vehicle types parsed, but vehicle_types list inside data was null.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Vehicle Types Validation Error: {ex.Message}");
            }
            return null;
        }
    }
}