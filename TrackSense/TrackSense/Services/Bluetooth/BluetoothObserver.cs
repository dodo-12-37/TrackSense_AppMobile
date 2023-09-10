using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackSense.Services.Bluetooth
{
    public class BluetoothObserver : IObserver<BluetoothEvent>
    {
        private IDisposable _unsubscribe;
        private Action<BluetoothEvent> _action;

        public BluetoothObserver(IObservable<BluetoothEvent> bluetoothService, Action<BluetoothEvent> action)
        {
            if (bluetoothService == null)
            {
                throw new ArgumentNullException(nameof(bluetoothService));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _action = action;
            _unsubscribe = bluetoothService.Subscribe(this);
        }
        public void OnCompleted()
        {
            Unsubscribe();
        }

        public void Unsubscribe()
        {
            _unsubscribe?.Dispose();
            _unsubscribe = null;
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(BluetoothEvent value)
        {
            _action(value);
        }
    }
}
