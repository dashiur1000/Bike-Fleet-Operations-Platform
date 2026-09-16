using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aspAPI.Models
{
    public class StationStatusDto
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        [Required]
        public string station_id { get; set; } = string.Empty;
        public int num_vehicles_available { get; set; }
        public int num_docks_available { get; set; }
        public int is_renting { get; set; }
        public int is_returning { get; set; }
        public int last_reported { get; set; }
    }
}
