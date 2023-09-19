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
        public string StartedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public int Distance { get; set; }

        public CompletedRideSummary(Entities.CompletedRideSummary entite)
        {
            if (entite is null)
            {
                throw new ArgumentNullException(nameof(entite));
            }

            this.CompletedRideId = entite.CompletedRideId;
            this.PlannedRideName = entite.PlannedRideName;
            this.StartedAt = entite.StartedAt.ToString();
            this.Duration = entite.Duration;
            this.Distance = entite.Distance;
        }
    }
}
