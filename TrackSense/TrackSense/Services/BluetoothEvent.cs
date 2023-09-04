namespace TrackSense.Services
{
    public class BluetoothEvent
    {
        public BluetoothEventType Type { get; set;}
        public string rideData { get; set;}
        public bool isConnected { get; set;}
        public BluetoothEvent(BluetoothEventType eventType, bool isConnected, string rideData = null)
        {
            this.rideData = rideData;
            this.isConnected = isConnected;
        }

    }
}