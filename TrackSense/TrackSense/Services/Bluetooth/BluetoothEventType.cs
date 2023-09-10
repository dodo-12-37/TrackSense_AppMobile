using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackSense.Services.Bluetooth
{
    public enum BluetoothEventType
    {
        CONNECTION,
        DECONNECTION,
        SENDING_RIDE_STATS,
        SENDING_RIDE_POINT
    }
}
