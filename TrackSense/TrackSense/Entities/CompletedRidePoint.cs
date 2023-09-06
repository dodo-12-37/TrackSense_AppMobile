namespace TrackSense.Entities
{
    public class CompletedRidePoint
    {
        public Location Location { get; set; }
        public int RideStep { get; set; }
        public double Temperature { get; set; }
        public DateTime DateTime { get; set; }

        public CompletedRidePoint(Location location, int rideStep, double temperature, DateTime dateTime)
        {
            Location = location;
            RideStep = rideStep;
            Temperature = temperature;
            DateTime = dateTime;
        }
    }
}