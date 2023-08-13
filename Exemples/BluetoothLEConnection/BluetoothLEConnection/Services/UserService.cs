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

            // Online
            var response = await httpClient.GetAsync("https://localhost:7188/api/users/" + p_userId);
            if (response.IsSuccessStatusCode)
            {
                this.User = await response.Content.ReadFromJsonAsync(UserContext.Default.User);
            }

            // Offline
            //using var stream = await FileSystem.OpenAppPackageFileAsync("userdata.json");
            //using var reader = new StreamReader(stream);
            //var contents = await reader.ReadToEndAsync();
            //UsersList = JsonSerializer.Deserialize(contents, UserContext.Default.ListUser);

            return this.User;
        }

        public async Task<List<User>> GetUsers()
        {
            //if (UsersList?.Count > 0)
            //{
            //    return UsersList;
            //}


            // Online
            //var response = await httpClient.GetAsync("https://localhost:7188/api/users");
            //var response = await httpClient.GetAsync("https://192.168.2.76:7188/api/users");
            var response = await httpClient.GetAsync("http://192.168.2.76:5267/api/users");
            //var response = await httpClient.GetAsync("http://192.168.2.76:5267/api/users/2");
            if (response.IsSuccessStatusCode)
            {
                var test = await response.Content.ReadFromJsonAsync(UserContext.Default.User);
                this.UsersList = response.Content.ReadFromJsonAsync(UserContext.Default.ListUser).Result;
                //this.UsersList = response.Content.ReadFromJsonAsync(UserContext.Default.ListUser).Result;
                var res = response.Content.ReadAsStringAsync().Result;

                //this.User = await response.Content.ReadFromJsonAsync(UserContext.Default.User);
            }

            // Offline
            //using var stream = await FileSystem.OpenAppPackageFileAsync("userdata.json");
            //using var reader = new StreamReader(stream);
            //var contents = await reader.ReadToEndAsync();
            //UsersList = JsonSerializer.Deserialize(contents, UserContext.Default.ListUser);

            return this.UsersList;
        }
    }
}
