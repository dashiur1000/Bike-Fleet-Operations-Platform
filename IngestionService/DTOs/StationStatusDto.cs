using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestionService.DTOs
{
    public class StationStatusDto
    {
        [Required]
        public string station_id { get; set; } = string.Empty;
        public int num_vehicles_available { get; set; }
        public int num_docks_available { get; set; }
        public bool is_renting { get; set; }
        public bool is_returning { get; set; }
        public bool last_reported { get; set; }
    }
}
