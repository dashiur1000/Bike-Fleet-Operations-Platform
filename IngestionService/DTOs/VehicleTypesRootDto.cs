using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IngestionService.DTOs
{
    public class VehicleTypesRootDto
    {
        [JsonPropertyName("last_updated")]
        public long last_updated { get; set; }

        [JsonPropertyName("ttl")]
        public int ttl { get; set; }

        [JsonPropertyName("data")]
        public VehicleTypesDataDto data { get; set; }
    }

    public class VehicleTypesDataDto
    {
        [JsonPropertyName("vehicle_types")]
        public List<VehicleTypesDto> vehicle_types { get; set; } = new();
    }
}