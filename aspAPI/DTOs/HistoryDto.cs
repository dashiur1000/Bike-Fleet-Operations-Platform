namespace aspAPI.DTOs
{
    public class HistoryDto
    {
        public DateTime timestamp {  get; set; }
        public int availableBikes { get; set; }
        public int availableDocks { get; set; }
    }
}
