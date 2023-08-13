//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using BluetoothLEConnection.Model;
//using BluetoothLEConnection.View;
//using BluetoothLEConnection.Services;

//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using Microsoft.Maui.Networking;
//using System.Diagnostics;
//using Plugin.BLE.Abstractions.Contracts;


//namespace BluetoothLEConnection.ViewModel
//{
//    public partial class UserViewModel : BaseViewModel
//    {
//        public ObservableCollection<User> Users { get; } = new();
//        UserService userService;
//        IConnectivity connectivity;

//        public UserViewModel(UserService userService, IConnectivity connectivity)
//        {
//            Title = "User Finder";
//            this.userService = userService;
//            this.connectivity = connectivity;
//        }


//        [ObservableProperty]
//        bool isRefreshing;


//        [RelayCommand]
//        async Task GetUsersAsync()
//        {
//            if (IsBusy)
//            {
//                return;
//            }

//            try
//            {
//                if (connectivity.NetworkAccess != NetworkAccess.Internet)
//                {
//                    await Shell.Current.DisplayAlert("No connectivity!",
//                        $"Please check internet and try again.", "OK");
//                    return;
//                }

//                IsBusy = true;

//                if (Users.Count != 0)
//                {
//                    Users.Clear();
//                }

//                var users = await userService.GetUsers();
//                await Shell.Current.DisplayAlert("User infos", $"Test", "OK");

//                foreach (var user in users)
//                {
//                    Users.Add(user);
//                    await Shell.Current.DisplayAlert("User infos", $"The user number {Users.Count} in database is : {user}.", "OK");
//                }

//                await Shell.Current.DisplayAlert("Number of users", $"Number of users in database is {Users.Count}.", "OK");

//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine($"Unable to get users: {ex.Message}");
//                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
//            }
//            finally
//            {
//                IsBusy = false;
//                IsRefreshing = false;
//            }
//        }


//    }
//}


/*
//    x:DataType="viewmodel:BluetoothDeviceViewModel"

//*/