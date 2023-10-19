using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackSense.Services.API.APIDTO
{
    internal class UserDTO
    {
        public string UserLogin { get; set; }

        public string Password { get; set; }

        public string PasswordConfirmed { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public UserDTO()
        {
            ;
        }

        public UserDTO(Entities.User entite)
        {
            UserLogin = entite.UserLogin ?? throw new ArgumentNullException(nameof(entite.UserLogin));
            Password = entite.Password ?? throw new ArgumentNullException(nameof(entite.Password));
            PasswordConfirmed = entite.PasswordConfirmed ?? throw new ArgumentNullException(nameof(entite.PasswordConfirmed));
            Email = entite.Email ?? throw new ArgumentNullException(nameof(entite.Email));
            FirstName = entite.FirstName ?? throw new ArgumentNullException(nameof(entite.FirstName));
            LastName = entite.LastName ?? throw new ArgumentNullException(nameof(entite.LastName));
        }

        public Entities.User ToEntity()
        {
            return new Entities.User()
            {
                UserLogin = this.UserLogin,
                Password = this.Password,
                PasswordConfirmed = this.PasswordConfirmed,
                Email = this.Email,
                FirstName = this.FirstName,
                LastName = this.LastName
            };
        }
    }
}
