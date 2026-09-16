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
    public class VehicleTypesHandler
    {
        private readonly SqlDbContext _db;
        public VehicleTypesHandler(SqlDbContext db)
        {
            _db = db;
        }
        public async Task HandleAsync(string vehicleTypeJson)
        {
            try
            {
                var incomingVehicle = JsonSerializer.Deserialize<VehicleTypesDto>(vehicleTypeJson);
                if (incomingVehicle == null || string.IsNullOrEmpty(incomingVehicle.vehicle_type_id))
                {
                    return;
                }
                var existingVehicle = await _db.vehicles
                    .FirstOrDefaultAsync(s => s.vehicle_type_id == incomingVehicle.vehicle_type_id);
                if (existingVehicle == null)
                {
                    _db.vehicles.Add(incomingVehicle);
                }
                else
                {
                    existingVehicle.form_factor = incomingVehicle.form_factor;
                    existingVehicle.propulsion_type = incomingVehicle.propulsion_type;

                    _db.vehicles.Update(existingVehicle);
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
