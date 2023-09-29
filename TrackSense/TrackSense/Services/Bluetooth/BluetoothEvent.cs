using TrackSense.Entities;

namespace TrackSense.Services.Bluetooth
{
    public class BluetoothEvent
    {
        public BluetoothEventType Type { get; set; }
        public CompletedRide RideData { get; set; }
        public CompletedRidePoint RidePoint { get; set; }

        public BluetoothEvent(BluetoothEventType eventType, bool isConnected)
        {
            Type = eventType;
        }
        public BluetoothEvent(BluetoothEventType eventType, CompletedRide completedRide)
        {
            Type = eventType;
            RideData = completedRide;
        }

        public BluetoothEvent(BluetoothEventType eventType, CompletedRidePoint completedRidePoint)
        {
            Type = eventType;
            RidePoint = completedRidePoint;
        }
    }
}