namespace Flight_delay_analyzer.Storage
{
    public class JSONInputs
    {
        public string OriginAirport { get; set; }
        public string DestinationAirport { get; set; }
        public DateTime dateOfFlight => DateTime.Today.AddDays(-1);
    }
}