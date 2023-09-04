namespace TrackSense.Services
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
            this.observers.Remove(this.observer);
        }
    }
}