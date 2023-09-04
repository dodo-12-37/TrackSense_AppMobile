using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TrackSense.Models;

namespace TrackSense.Data
{
    public class RideData
    {
        private string _fileName = Path.Combine(FileSystem.AppDataDirectory, "rides.json");

        public RideData()
        {
            ;
        }

        public void AddRide(CompletedRide completedRide)
        {
            if (completedRide is null)
            {
                throw new ArgumentNullException(nameof(completedRide));
            }

            throw new NotImplementedException();
        }
    }
}
