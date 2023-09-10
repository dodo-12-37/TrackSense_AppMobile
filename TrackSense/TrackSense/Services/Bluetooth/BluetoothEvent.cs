using TrackSense.Entities;

namespace TrackSense.Services.Bluetooth
{
    public class BluetoothEvent
    {
        public BluetoothEventType Type { get; set; }
        public CompletedRide RideData { get; set; }
        public List<CompletedRidePoint> RidePoints { get; set; }
        public bool IsConnected { get; set; }

        public BluetoothEvent(BluetoothEventType eventType, bool isConnected)
        {
            Type = eventType;
            IsConnected = isConnected;
        }
        public BluetoothEvent(BluetoothEventType eventType, bool isConnected, CompletedRide completedRide)
        {
            Type = eventType;
            RideData = completedRide;
            IsConnected = isConnected;
        }

        public BluetoothEvent(BluetoothEventType eventType, bool isConnected, List<CompletedRidePoint> completedRidePoints)
        {
            Type = eventType;
            RidePoints = completedRidePoints;
            IsConnected = isConnected;
        }

    }
}