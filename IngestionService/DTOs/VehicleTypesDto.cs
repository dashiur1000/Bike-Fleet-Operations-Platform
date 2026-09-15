using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IngestionService.DTOs
{
    public class VehicleTypesDto
    {
        [JsonPropertyName("vehicle_type_id")]
        public string vehicle_type_id { get; set; } = string.Empty;

        [JsonPropertyName("form_factor")]
        public string form_factor { get; set; } = string.Empty;

        [JsonPropertyName("propulsion_type")]
        public string propulsion_type { get; set; } = string.Empty;
    }
}
