using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackSense.Entities
{
    public class CompletedRide
    {
        public Guid Id { get; set; }

        public CompletedRide(Guid id)
        {
            this.Id = Id;
        }
    }
}
