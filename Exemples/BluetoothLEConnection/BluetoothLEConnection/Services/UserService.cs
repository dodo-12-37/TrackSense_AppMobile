using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http.Json;
using BluetoothLEConnection.Model;
//using CoreML;
using System.Text.Json;

namespace BluetoothLEConnection.Services
{
    public class UserService
    {
        HttpClient httpClient;
        public List<User> UsersList;
        public User User;
        public string IP = "192.168.1.57";  // IP de mon PC. À changer selon votre IP.

        public UserService()
        {
            this.httpClient = new HttpClient();
            this.UsersList = new List<User>();
            this.User = null;
            

            //Users = new List<User>();
            //Users.Add(new User() {
            //    UserId = 0,
            //    UserName = "Jean",
            //    UserAddress = "1, rue de la paix, Chicago, Il, É-U",
            //    UserCodePostal = "60304",
            //    UserEmail = "Jean@email.com"
            //});
        }

        public async Task<User> GetUser(int p_userId)
        {
            if (p_userId <= 0 || p_userId > 3)
            {
                throw new ArgumentException("Invalid user id");
            }

            var response = await httpClient.GetAsync($"https://{this.IP}:7188/api/users/" + p_userId);
            if (response.IsSuccessStatusCode)
            {
                this.User = await response.Content.ReadFromJsonAsync(UserContext.Default.User);
            }

            return this.User;
        }

        public async Task<List<User>> GetUsers()
        {
            var response = await httpClient.GetAsync($"http://{this.IP}:5267/api/users");
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                this.UsersList = await response.Content.ReadFromJsonAsync<List<User>>(options);
            }

            return this.UsersList;
        }


        public async Task<bool> PostUser()
        {
            Random rnd = new Random();
            int num = rnd.Next();

            this.User = new User()
            {
                UserId = 0,
                UserName = "Jean",
                UserAddress = "1, rue de la paix, Chicago, Il, É-U",
                UserCodePostal = "60304",
                UserEmail = $"Jean{num}@email.com"
            };

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
     
            var response = await httpClient.PostAsJsonAsync($"http://{this.IP}:5267/api/users", this.User, options);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
    }
}
