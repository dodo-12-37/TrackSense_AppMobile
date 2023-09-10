namespace TrackSense.Services.Bluetooth
{
    internal class UnsubscriberBluetooth : IDisposable
    {
        private IObserver<BluetoothEvent> observer;
        private List<IObserver<BluetoothEvent>> observers;

        public UnsubscriberBluetooth(List<IObserver<BluetoothEvent>> observers, IObserver<BluetoothEvent> observer)
        {
            this.observers = observers;
            this.observer = observer;
        }
        public void Dispose()
        {
            observers.Remove(observer);
        }
    }
}