using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantZamaApp.Models;
using RestaurantZamaApp.Services;
using RestaurantZamaApp.Views;
using System.Text.Json;

namespace RestaurantZamaApp.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private RegisterModel registerModel;
        [ObservableProperty]
        private LoginModel loginModel;

        [ObservableProperty]
        private string userName;
        [ObservableProperty]
        private bool isAuthenticated;

        // New properties for special of the day
        [ObservableProperty]
        private string specialOfTheDay;
        [ObservableProperty]
        private decimal specialPrice;

        private readonly ClientService clientService;

        public MainPageViewModel(ClientService clientService)
        {
            this.clientService = clientService;
            RegisterModel = new();
            LoginModel = new();
            IsAuthenticated = false;
            GetUserNameFromSecuredStorage();
            LoadSpecialOfTheDay();
        }

        [RelayCommand]
        private async Task Register()
        {
            await clientService.Register(RegisterModel);
        }

        [RelayCommand]
        private async Task Login()
        {
            await clientService.Login(LoginModel);
            GetUserNameFromSecuredStorage();
        }

        [RelayCommand]
        private async Task Logout()
        {
            SecureStorage.Default.Remove("Authentication");
            IsAuthenticated = false;
            UserName = "Guest";
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task GoToWeatherForecast()
        {
            await Shell.Current.GoToAsync(nameof(WeatherForecastPage));
        }

        // New commands for restaurant features
        [RelayCommand]
        private async Task GoToProfile()
        {
            if (!IsAuthenticated)
            {
                await Shell.Current.DisplayAlert("Error", "Please login to view profile", "OK");
                return;
            }
            await Shell.Current.GoToAsync(nameof(ProfilePage));
        }
        [RelayCommand]
        private async Task MakeReservation()
        {
            if (!IsAuthenticated)
            {
                await Shell.Current.DisplayAlert("Error", "Please login to make a reservation", "OK");
                return;
            }
            await Shell.Current.GoToAsync(nameof(ReservationsPage));
        }

        [RelayCommand]
        private async Task ViewMenu()
        {
            // Navigate to menu page
            await Shell.Current.GoToAsync(nameof(MenuPage));
        }

        private async void GetUserNameFromSecuredStorage()
        {
            if (!string.IsNullOrEmpty(UserName) && userName! != "Guest")
            {
                IsAuthenticated = true;
                return;
            }
            var serializedLoginResponseInStorage = await SecureStorage.Default.GetAsync("Authentication");
            if (serializedLoginResponseInStorage != null)
            {
                UserName = JsonSerializer.Deserialize<LoginResponse>(serializedLoginResponseInStorage)!.UserName!;
                IsAuthenticated = true;
                return;
            }
            UserName = "Guest";
            IsAuthenticated = false;
        }

        private void LoadSpecialOfTheDay()
        {
            // In a real app, this would load from a service
            SpecialOfTheDay = "Sarmale cu mămăligă";
            SpecialPrice = 25.99m;
        }
    }
}