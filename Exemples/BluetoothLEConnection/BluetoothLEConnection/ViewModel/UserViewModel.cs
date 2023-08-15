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
//                    await Shell.Current.DisplayAlert("Aucune connexion!",
//                        $"SVP, vérifier votre connexion internet et recommencer.", "OK");
//                    return;
//                }

//                IsBusy = true;

//                if (Users.Count != 0)
//                {
//                    Users.Clear();
//                }

//                var users = await userService.GetUsers();

//                foreach (var user in users)
//                {
//                    Users.Add(user);

//                    await Shell.Current.DisplayAlert("Infos des Users", $"L'Id numéro {Users.Count} dans la database est : \n{user}.", "OK");
//                }

//                await Shell.Current.DisplayAlert("Nombre de Users", $"Le nombre total de Users dans la database est : {Users.Count}.", "OK");

//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine($"Impossible d'avoir les users : {ex.Message}");
//                await Shell.Current.DisplayAlert("Erreur!", ex.Message, "OK");
//            }
//            finally
//            {
//                IsBusy = false;
//                IsRefreshing = false;
//            }
//        }

//        [RelayCommand]
//        async Task CreateUsersAsync()
//        {
//            if (IsBusy)
//            {
//                return;
//            }

//            try
//            {
//                if (connectivity.NetworkAccess != NetworkAccess.Internet)
//                {
//                    await Shell.Current.DisplayAlert("Aucune connexion!",
//                        $"SVP, vérifier votre connexion internet et recommencer.", "OK");
//                    return;
//                }

//                IsBusy = true;

//                bool isCreated = await userService.PostUser();

//                if (isCreated)
//                {
//                    await Shell.Current.DisplayAlert("Création d'un User", $"L'utilisateur à bien été créer!", "OK");
//                }
//                else
//                {
//                    await Shell.Current.DisplayAlert("Création d'un User", $"L'utilisateur n'a pas été créer...", "OK");
//                }

//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine($"Impossible d'avoir les users : {ex.Message}");
//                await Shell.Current.DisplayAlert("Erreur!", ex.Message, "OK");
//            }
//            finally
//            {
//                IsBusy = false;
//                IsRefreshing = false;
//            }
//        }


//    }
//}