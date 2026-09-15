using System.Collections.Generic;
using IngestionService.DTOs;

namespace IngestionService.DTOs
{
    public class StationStatusRootDto
    {
        public long last_updated { get; set; }
        public int ttl { get; set; }
        public StationStatusDataDto data { get; set; } = new();
    }

    public class StationStatusDataDto
    {
        public List<StationStatusDto> stations { get; set; } = new();
    }
}