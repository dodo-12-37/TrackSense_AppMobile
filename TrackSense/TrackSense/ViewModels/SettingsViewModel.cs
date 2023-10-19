using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackSense.Configurations;
using TrackSense.Entities;
using TrackSense.Services;
using TrackSense.Services.Bluetooth;

namespace TrackSense.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        RideService _rideService;
        UserService _userService;
        BluetoothService _bluetoothService;
        IConfigurationManager _configuration;

        public SettingsViewModel(RideService rideService, BluetoothService bluetoothService, UserService userService, IConfigurationManager configurationManager)
        {
            Title = "Paramètres";
            _rideService = rideService;
            _bluetoothService = bluetoothService;
            _userService = userService;
            _configuration = configurationManager;
        }

        [RelayCommand]
        async Task DeleteRidesFromStorageAsync()
        {
            List<CompletedRide> allRides = _rideService.GetAllCompletedRides();
            int totalNumberOfRides = allRides.Count;

            bool choice = await Shell.Current.DisplayAlert("Supprimer les trajets", $"Cette action supprimera {totalNumberOfRides} trajet(s) enregistré(s) dans la mémoire du téléphone. Êtes-vous sûr ?", "Oui", "Non");
            if (choice)
            {
                _rideService.DeleteRidesFromLocalStorage();
            }
        }

        [RelayCommand]
        async Task DisconnectFromDeviceAsync()
        {
            this._bluetoothService.DisconnectDevice();
        }

        [RelayCommand]
        async Task DisplayApiUrlOptionsAsync()
        {
            Settings settings = _configuration.LoadSettings();
            Shell.Current.CurrentPage.FindByName<Editor>("apiUrlEditor").Text = settings.ApiUrl;
            Shell.Current.CurrentPage.FindByName<Grid>("apiUrlOptions").IsVisible = true;
        }

        [RelayCommand]
        async Task ChangeApiUrlAsync()
        {
            string modifiedApiUrl = Shell.Current.CurrentPage.FindByName<Editor>("apiUrlEditor").Text;

            Uri uriResult;
            bool uriIsValid = Uri.TryCreate(modifiedApiUrl, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (!uriIsValid)
            {
                await Shell.Current.DisplayAlert("Oups", "Le format de votre URL n'est pas valide", "Ok");
                return;
            }

            Settings settings = _configuration.LoadSettings();

            if (modifiedApiUrl != settings.ApiUrl)
            {
                settings.ApiUrl = modifiedApiUrl;

                _configuration.SaveSettings(settings);

                await Shell.Current.DisplayAlert("Modification URL", $"Modification de l'URL de l'API pour {modifiedApiUrl}", "Ok");
            }


            Shell.Current.CurrentPage.FindByName<Grid>("apiUrlOptions").IsVisible = false;

        }

        [RelayCommand]
        async Task DisplayUsernameOptionsAsync()
        {
            Settings settings = _configuration.LoadSettings();
            Shell.Current.CurrentPage.FindByName<Editor>("usernameEditor").Text = settings.Username;
            Shell.Current.CurrentPage.FindByName<Grid>("usernameOptions").IsVisible = true;
        }

        [RelayCommand]
        async Task ChangeUsernameAsync()
        {
            string modifiedUsername = Shell.Current.CurrentPage.FindByName<Editor>("usernameEditor").Text;

            string pattern = @"[^a-zA-Z0-9à-ÿ_\-]"; // Allow letters, digits, underscores, hyphens, and accented characters

            string sanitizedUsername = Regex.Replace(modifiedUsername, pattern, "");

            if (String.IsNullOrWhiteSpace(sanitizedUsername))
            {
                await Shell.Current.DisplayAlert("Oups", "Le format de votre nom utilisateur n'est pas valide", "Ok");
                return;
            }

            Settings settings = _configuration.LoadSettings();

            if (sanitizedUsername != settings.Username)
            {
                settings.Username = sanitizedUsername;

                _configuration.SaveSettings(settings);

                await Shell.Current.DisplayAlert("Modification URL", $"Votre nom d'utilisateur a été modifié pour {sanitizedUsername}", "Ok");
            }


            Shell.Current.CurrentPage.FindByName<Grid>("usernameOptions").IsVisible = false;
        }

        [RelayCommand]
        async Task DisplayAccountOptionsAsync()
        {
            Settings settings = _configuration.LoadSettings();
            Shell.Current.CurrentPage.FindByName<Grid>("accountOptions").IsVisible = true;
        }

        [RelayCommand]
        async Task CreateAccountAsync()
        {
            string accountUsername = Shell.Current.CurrentPage.FindByName<Editor>("accountEditor").Text;

            string pattern = @"[^a-zA-Z0-9à-ÿ_\-]"; // Allow letters, digits, underscores, hyphens, and accented characters

            string sanitizedUsername = Regex.Replace(accountUsername, pattern, "");

            if (String.IsNullOrWhiteSpace(sanitizedUsername))
            {
                await Shell.Current.DisplayAlert("Oups", "Le format de votre nom utilisateur n'est pas valide", "Ok");
                return;
            }

            User user = new User()
            {
                UserLogin = sanitizedUsername,
                Password = sanitizedUsername,
                PasswordConfirmed = sanitizedUsername,
                Email = "sanitizedUsername@email.com",
                FirstName = "sanitizedUsername",
                LastName = "sanitizedUsername"
            };

            HttpResponseMessage response = await _userService.CreateUser(user);

            if (!response.IsSuccessStatusCode)
            {
                await Shell.Current.DisplayAlert("Création de compte", $"Une erreur est survenue lors de la création de votre compte. Le nom d'utilisateur existe déjà ou le format n'est pas valide", "Ok");
                return;
            }

            Settings userSettings = _configuration.LoadSettings();
            userSettings.Username = sanitizedUsername;
            _configuration.SaveSettings(userSettings);

            await Shell.Current.DisplayAlert("Création de compte", $"Votre compte a été créé avec succès. Vous êtes connecté en tant que {sanitizedUsername}", "Ok");
            Shell.Current.CurrentPage.FindByName<Grid>("accountOptions").IsVisible = false;
        }
    }
}
