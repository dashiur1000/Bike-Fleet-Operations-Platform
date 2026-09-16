using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aspAPI.Models
{
    public class StationInformationDto
    {
        [Required]
        public string station_id { get; set; } = string.Empty;
        public string short_name { get; set; } = string.Empty;
        public double lon { get; set; }
        public double lat { get; set; }
        public int capacity { get; set; }
    }
}
