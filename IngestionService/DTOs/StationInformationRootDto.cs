using System.Collections.Generic;

namespace IngestionService.DTOs
{
    public class StationInformationRootDto
    {
        public long last_updated { get; set; }
        public int ttl { get; set; }
        public StationInformationDataDto data { get; set; } = new();
    }

    public class StationInformationDataDto
    {
        public List<StationInformationDto> stations { get; set; } = new();
    }
}