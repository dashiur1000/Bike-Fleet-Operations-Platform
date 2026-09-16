namespace aspAPI.DTOs
{
    public class StationsDto
    {
        public string station_id { get; set; } = string.Empty;
        public string short_name { get; set; } = string.Empty;
        public double lon { get; set; }
        public double lat { get; set; }
        public int capacity { get; set; }
        public int num_vehicles_available { get; set; }
        public int num_docks_available { get; set; }
        public int is_renting { get; set; }
        public int is_returning { get; set; }
        public int last_reported { get; set; }
    }
}
