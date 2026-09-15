using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestionService.DTOs
{
    public class VehicleTypesDto
    {
        public string vehicle_type_id { get; set; } = string.Empty;
        public string form_factor { get; set; } = string.Empty;
        public string propulsion_type {  get; set; } = string.Empty;
    }
}
