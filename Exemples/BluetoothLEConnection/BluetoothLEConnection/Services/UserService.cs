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
        public string Protocole = "http";  // À changer selon communication HTTP(5267) ou HTTPS(7188)
        public string IP = "192.168.2.76";  // IP de mon PC. À changer selon votre IP.
        public string Port = "5267";  // Port de mon PC. À changer selon communication HTTP(5267) ou HTTPS(7188)

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
            await this.GetUsers();

            if (p_userId <= 0 || p_userId > this.UsersList.Count)
            {
                throw new ArgumentException("Invalid user id");
            }

            var response = await httpClient.GetAsync($"{this.Protocole}://{this.IP}:{this.Port}/api/users/" + p_userId);
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                this.User = await response.Content.ReadFromJsonAsync<User>(options);
            }

            return this.User;
        }

        public async Task<List<User>> GetUsers()
        {
            var response = await httpClient.GetAsync($"{this.Protocole}://{this.IP}:{this.Port}/api/users");
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


        public async Task<User> PostUser(string p_userName)
        {
            Random rnd = new Random();
            int randomNumber = rnd.Next();

            this.User = new User()
            {
                UserId = 0,
                UserName = $"{p_userName}_{randomNumber}",
                UserAddress = "1, rue de la paix, Chicago, Il, É-U",
                UserCodePostal = "60304",
                UserEmail = $"PFL_{randomNumber}@email.com"
            };

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
     
            var response = await httpClient.PostAsJsonAsync($"{this.Protocole}://{this.IP}:{this.Port}/api/users", this.User, options);

            var newUser = response.Content.ReadFromJsonAsync<User>(options).Result;

            return newUser;
        }
    }
}
