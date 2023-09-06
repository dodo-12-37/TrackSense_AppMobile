using TrackSense.Entities;

namespace TrackSense.Data.DTO;

public class CompletedRideDTO
{
    public Guid Id { get; set; }

    public CompletedRideDTO()
    {
        ;
    }

    public CompletedRideDTO(CompletedRide entite)
    {
        if (entite is null)
        {
            throw new ArgumentNullException(nameof(entite));
        }

        this.Id = entite.Id;
    }

    public CompletedRide ToEntity()
    {
        return new CompletedRide(this.Id);
    }

}
