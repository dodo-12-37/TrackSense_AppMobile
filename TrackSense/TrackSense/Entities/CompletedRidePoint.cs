namespace TrackSense.Entities
{
    public class CompletedRidePoint
    {
        public int RideStep { get; set; }
        public Location Location { get; set; }
        public double Temperature { get; set; }
        public TimeSpan EffectiveTime { get; set; }

        public CompletedRidePoint(int rideStep, Location location, double temperature, TimeSpan effectiveTime)
        {
            RideStep = rideStep;
            Location = location;
            Temperature = temperature;
            EffectiveTime = effectiveTime;
        }
    }
}