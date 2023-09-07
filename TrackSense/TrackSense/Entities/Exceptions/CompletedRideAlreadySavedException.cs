using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackSense.Entities.Exceptions
{
    internal class CompletedRideAlreadySavedException : InvalidOperationException
    {
        public CompletedRideAlreadySavedException()
        {
        }

        public CompletedRideAlreadySavedException(string message) : base(message)
        {
        }

        public CompletedRideAlreadySavedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
