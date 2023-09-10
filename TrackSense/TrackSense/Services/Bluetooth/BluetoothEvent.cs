using TrackSense.Entities;

namespace TrackSense.Services.Bluetooth
{
    public class BluetoothEvent
    {
        public BluetoothEventType Type { get; set; }
        public CompletedRide RideData { get; set; }
        public CompletedRidePoint RidePoint { get; set; }
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

        public BluetoothEvent(BluetoothEventType eventType, bool isConnected, CompletedRidePoint completedRidePoint)
        {
            Type = eventType;
            RidePoint = completedRidePoint;
            IsConnected = isConnected;
        }

    }
}