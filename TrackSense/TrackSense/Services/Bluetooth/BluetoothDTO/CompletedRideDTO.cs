using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackSense.Entities;

namespace TrackSense.Services.Bluetooth.BluetoothDTO
{
    public class CompletedRideDTO
    {
        public Guid CompletedRideId { get; set; }
        public Guid? PlannedRideId { get; set; }
        public List<CompletedRidePointDTO> CompletedRidePoints { get; set; } = new();
        public CompletedRideStatisticsDTO Statistics { get; set; }

        public CompletedRideDTO(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            message = message.Replace('.', ',');

            string[] values = message.Split(';');

            this.CompletedRideId = Guid.Parse(values[0]);
            string plannedRideId = values[1];
            this.PlannedRideId = plannedRideId == "N" ? null : Guid.Parse(values[1]);
            double maxSpeed = double.Parse(values[2]);
            double avgSpeed = double.Parse(values[3]);
            double distance = double.Parse(values[4]);
            TimeSpan duration = TimeSpan.FromMinutes(double.Parse(values[5]));
            DateTime dateBegin = DateTime.Parse(values[6]);
            DateTime dateEnd = DateTime.Parse(values[7]);
            int nbOfPoints = int.Parse(values[8]);
            int nbOfFalls = int.Parse(values[9]);
            int calories = 0;

            this.Statistics = new CompletedRideStatisticsDTO(maxSpeed, avgSpeed, distance, duration, nbOfPoints, calories, nbOfFalls);
        }

        internal CompletedRide ToEntity()
        {
            List<CompletedRidePoint> listPoints = this.CompletedRidePoints.Select(pointDTO
                => pointDTO.ToEntity()).ToList();

            return new CompletedRide()
            {
                CompletedRideId = this.CompletedRideId,
                PlannedRideId = this.PlannedRideId,
                CompletedRidePoints = listPoints,
                Statistics = this.Statistics.ToEntity()
            };
        }
    }
}
