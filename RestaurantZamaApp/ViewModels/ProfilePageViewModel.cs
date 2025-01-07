using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using RestaurantZamaApp.Models;
using RestaurantZamaApp.Services;

namespace RestaurantZamaApp.ViewModels
{
    public partial class ProfilePageViewModel : ObservableObject
    {
        private readonly IProfileService _profileService;
        private readonly ILogger<ProfilePageViewModel> _logger;

        [ObservableProperty]
        private User currentUser;

        [ObservableProperty]
        private ObservableCollection<User> allProfiles = new();

        [ObservableProperty]
        private bool isBusy;

        public ProfilePageViewModel(
            IProfileService profileService,
            ILogger<ProfilePageViewModel> logger)
        {
            _profileService = profileService;
            _logger = logger;
        }

        [RelayCommand]
        public async Task LoadUserData()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await LoadAllProfilesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user data");
                await Shell.Current.DisplayAlert("Eroare", "Nu s-au putut încărca datele utilizatorului", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadAllProfilesAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                var profiles = await _profileService.GetAllProfilesAsync();
                AllProfiles.Clear();
                foreach (var profile in profiles)
                {
                    AllProfiles.Add(profile);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading profiles");
                await Shell.Current.DisplayAlert("Eroare", "Nu s-au putut încărca profilurile", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task UpdateProfile()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                if (CurrentUser?.Id == 0)
                {
                    await Shell.Current.DisplayAlert("Eroare", "Selectați un profil pentru actualizare", "OK");
                    return;
                }

                var updatedProfile = await _profileService.UpdateProfileAsync(CurrentUser.Id, CurrentUser);
                var index = AllProfiles.IndexOf(AllProfiles.FirstOrDefault(p => p.Id == updatedProfile.Id));
                if (index != -1)
                {
                    AllProfiles[index] = updatedProfile;
                }

                await Shell.Current.DisplayAlert("Succes", "Profil actualizat", "OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                await Shell.Current.DisplayAlert("Eroare", "Nu s-a putut actualiza profilul", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}