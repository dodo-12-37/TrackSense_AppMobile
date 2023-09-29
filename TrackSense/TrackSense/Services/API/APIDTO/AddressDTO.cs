using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackSense.Entities;

namespace TrackSense.Services.API.APIDTO
{
    public class AddressDTO
    {
        public int AddressId { get; set; }

        public string  LocationId { get; set; }

        public string AppartmentNumber { get; set; }

        public string StreetNumber { get; set; }

        public string StreetName { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }
        public virtual LocationDTO Location { get; set; }
        public AddressDTO()
        {

        }
        public Entities.Address ToEntity()
        {
            return new Entities.Address()
            {
                AddressId = this.AddressId,
                StreetNumber = this.StreetNumber,
                StreetName = this.StreetName,
                ZipCode = this.ZipCode,
                City = this.City,
                State = this.State,
                Country = this.Country,
                Location = this.Location.ToEntity()
            };
        }
        public AddressDTO(Entities.Address p_address)
        {
            if (p_address == null)
            {
                throw new ArgumentNullException(nameof(p_address));
            }
            this.AddressId = p_address.AddressId;
            this.AppartmentNumber = p_address.AppartmentNumber;
            this.StreetNumber = p_address.StreetNumber;
            this.StreetName = p_address.StreetName;
            this.ZipCode = p_address.ZipCode;
            this.City = p_address.City;
            this.State = p_address.State;
            this.Country = p_address.Country;
            this.Location = p_address.Location == null
                            ? null
                            : new LocationDTO(p_address.Location);
        }
    }
}
