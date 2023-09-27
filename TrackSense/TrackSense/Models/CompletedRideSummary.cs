using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackSense.Models
{
    public class CompletedRideSummary
    {
        public Guid CompletedRideId { get; set; }
        public string PlannedRideName { get; set; }
        public string StartedAtTime { get; set; }
        public string StartedAtDate { get; set; }

        public TimeSpan Duration { get; set; }
        public double Distance { get; set; }

        public CompletedRideSummary(Entities.CompletedRideSummary entite)
        {
            if (entite is null)
            {
                throw new ArgumentNullException(nameof(entite));
            }

            this.CompletedRideId = entite.CompletedRideId;
            this.PlannedRideName = entite.PlannedRideName;
            this.StartedAtDate = $"{entite.StartedAt.ToLongDateString()}";
            this.StartedAtTime = $"{entite.StartedAt.ToShortTimeString()}";
            this.Duration = entite.Duration;
            this.Distance = entite.Distance;
        }
    }
}
