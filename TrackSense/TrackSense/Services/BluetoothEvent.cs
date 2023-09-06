using TrackSense.Entities;

namespace TrackSense.Services
{
    public class BluetoothEvent
    {
        public BluetoothEventType Type { get; set;}
        public CompletedRide RideData { get; set;}
        public List<CompletedRidePoint> RidePoints { get; set;}
        public bool IsConnected { get; set;}

        public BluetoothEvent(BluetoothEventType eventType, bool isConnected)
        {
            this.Type = eventType;
            this.IsConnected = isConnected;
        }
        public BluetoothEvent(BluetoothEventType eventType, bool isConnected, CompletedRide completedRide)
        {
            this.Type = eventType;
            this.RideData = completedRide;
            this.IsConnected = isConnected;
        }

        public BluetoothEvent(BluetoothEventType eventType, bool isConnected, List<CompletedRidePoint> completedRidePoints)
        {
            this.Type = eventType;
            this.RidePoints = completedRidePoints;
            this.IsConnected = isConnected;
        }

    }
}