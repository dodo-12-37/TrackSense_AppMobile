using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TrackSense.Data.DTO;
using TrackSense.Entities;

namespace TrackSense.Data
{
    public class RideData
    {
        private string _fileName = Path.Combine(FileSystem.AppDataDirectory, "rides.json");

        public RideData()
        {
            if (!File.Exists(_fileName))
            {
                File.Create(_fileName);
            }
        }

        public void AddRide(CompletedRide completedRide)
        {
            if (completedRide is null)
            {
                throw new ArgumentNullException(nameof(completedRide));
            }

            if (GetCompletedRideById(completedRide.CompletedRideId) is not null)
            {
                throw new InvalidOperationException();
            }

            List<CompletedRideDTO> listeRidesDTO = ListRidesDTO();
            listeRidesDTO.Add(new CompletedRideDTO(completedRide)); //liste nulle

            SaveListInFile(listeRidesDTO);
        }

        private void SaveListInFile(List<CompletedRideDTO> listeRidesDTO)
        {
            if (listeRidesDTO is null)
            {
                throw new ArgumentNullException(nameof(listeRidesDTO));
            }

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            string chaineJson = JsonConvert.SerializeObject(listeRidesDTO, settings);
            File.WriteAllText(this._fileName, chaineJson);
        }

        private List<CompletedRideDTO> ListRidesDTO()
        {
            string chaineJson = File.ReadAllText(this._fileName);

            List<CompletedRideDTO> listCompletedRides = new List<CompletedRideDTO>();
            listCompletedRides = JsonConvert.DeserializeObject<List<CompletedRideDTO>>(chaineJson);

            return listCompletedRides;
        }

        public List<CompletedRide> ListCompletedRides()
        {
            List<CompletedRideDTO> completedRideDTOs = ListRidesDTO();
            List<CompletedRide> ridesList = new List<CompletedRide>();

            if (completedRideDTOs is not null)
            {
                ridesList = completedRideDTOs.Select(rideDTO => rideDTO.ToEntity()).ToList();
            }

            return ridesList;
        }

        private CompletedRide GetCompletedRideById(Guid id)
        {
            return this.ListCompletedRides().SingleOrDefault(ride => ride.CompletedRideId == id);
        }
    }
}
