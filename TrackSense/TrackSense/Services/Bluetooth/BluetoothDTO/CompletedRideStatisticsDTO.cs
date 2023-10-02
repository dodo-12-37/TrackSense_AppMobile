using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackSense.Entities;

namespace TrackSense.Services.Bluetooth.BluetoothDTO
{
    public class CompletedRideStatisticsDTO
    {
        public double MaximumSpeed { get; set; }
        public double AverageSpeed { get; set; }
        public double DistanceKm { get; set; }
        public TimeSpan Duration { get; set; }
        public int NumberOfPoints { get; set; }
        public int Calories { get; set; }
        public int Falls { get; set; }

        public CompletedRideStatisticsDTO(double maximumSpeed, double averageSpeed, double distance, TimeSpan duration, int numberOfPoints, int calories, int falls)
        {
            MaximumSpeed = maximumSpeed;
            AverageSpeed = averageSpeed;
            DistanceKm = distance;
            Duration = duration;
            NumberOfPoints = numberOfPoints;
            Calories = calories;
            Falls = falls;
        }

        internal CompletedRideStatistics ToEntity()
        {
            return new CompletedRideStatistics()
            {
                MaximumSpeed = this.MaximumSpeed,
                AverageSpeed = this.AverageSpeed,
                Distance = this.DistanceKm,
                Duration = this.Duration,
                NumberOfPoints = this.NumberOfPoints,
                Calories = this.Calories,
                Falls = this.Falls
            };
        }
    }
}
