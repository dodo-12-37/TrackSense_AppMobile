using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TrackSense.Data.DTO;
using TrackSense.Entities;
using TrackSense.Entities.Exceptions;

namespace TrackSense.Data
{
    public class RideData : ICompletedRideLocalData
    {
        private string _fileName = Path.Combine(FileSystem.AppDataDirectory, "rides.json");

        public RideData()
        {
            try
            {
                string jsonString = File.ReadAllText(this._fileName);
            }
            catch (FileNotFoundException)
            {
                SaveListInFile(new List<CompletedRideDTO>());
            }
            catch (PathTooLongException)
            {
                Debug.WriteLine("The path of the file is too long");
            }
        }

        public void AddCompletedRide(CompletedRide completedRide)
        {
            if (completedRide is null)
            {
                throw new ArgumentNullException(nameof(completedRide));
            }

            if (GetCompletedRideById(completedRide.CompletedRideId) is not null)
            {
                throw new CompletedRideAlreadySavedException("A completed ride with the same id already exists");
            }

            List<CompletedRideDTO> listeRidesDTO = ListRidesDTO();
            listeRidesDTO.Add(new CompletedRideDTO(completedRide));

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

            string jsonString = JsonConvert.SerializeObject(listeRidesDTO, settings);
            File.WriteAllText(this._fileName, jsonString);
        }

        private List<CompletedRideDTO> ListRidesDTO()
        {
            string jsonString = File.ReadAllText(this._fileName);

            List<CompletedRideDTO> listCompletedRides = new List<CompletedRideDTO>();
            if (!string.IsNullOrEmpty(jsonString))
            {
                listCompletedRides = JsonConvert.DeserializeObject<List<CompletedRideDTO>>(jsonString);
            }

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

        public CompletedRide GetCompletedRideById(Guid id)
        {
            return this.ListCompletedRides().SingleOrDefault(ride => ride.CompletedRideId == id);
        }

        public void DeleteCompletedRideById(Guid id)
        {
            List<CompletedRideDTO> completedRideDTOs = ListRidesDTO();
            CompletedRideDTO rideToDelete = completedRideDTOs.SingleOrDefault(ride => ride.CompletedRideId == id);
            if (rideToDelete is null)
            {
                throw new Exception("Ride not found");
            }
            else
            {
                completedRideDTOs.Remove(rideToDelete);
            }

            SaveListInFile(completedRideDTOs);
        }

        public void DeleteAllCompletedRides()
        {
            SaveListInFile(new List<CompletedRideDTO>());   
        }
    }
}
