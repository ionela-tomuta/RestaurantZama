using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using RestaurantZamaShared.Models;
using RestaurantZamaApp.Services;

namespace RestaurantZamaApp.ViewModels
{
    public partial class ProfilePageViewModel : ObservableObject
    {
        private readonly IProfileService _profileService;
        private readonly ILogger<ProfilePageViewModel> _logger;

        [ObservableProperty]
        private User _currentUser;

        [ObservableProperty]
        private bool _isBusy;

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
                int userId = Preferences.Get("UserId", 0);

                if (userId == 0)
                {
                    await Shell.Current.DisplayAlert("Eroare", "Utilizatorul nu este autentificat", "OK");
                    return;
                }

                CurrentUser = await _profileService.GetProfileByIdAsync(userId);
                if (CurrentUser == null)
                {
                    await Shell.Current.DisplayAlert("Eroare", "Nu s-au putut găsi datele utilizatorului", "OK");
                    return;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Eroare de comunicare cu serverul");
                await Shell.Current.DisplayAlert("Eroare de conexiune",
                    "Nu s-a putut stabili conexiunea cu serverul. Verificați conexiunea la internet.", "OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eroare neașteptată la încărcarea datelor");
                await Shell.Current.DisplayAlert("Eroare",
                    "A apărut o eroare neașteptată la încărcarea datelor", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SaveChanges()
        {
            if (IsBusy || CurrentUser == null) return;

            try
            {
                IsBusy = true;
                int userId = Preferences.Get("UserId", 0);

                if (userId == 0)
                {
                    await Shell.Current.DisplayAlert("Eroare", "Utilizatorul nu este autentificat", "OK");
                    return;
                }

                await _profileService.UpdateProfileAsync(userId, CurrentUser);
                await Shell.Current.DisplayAlert("Succes", "Modificările au fost salvate", "OK");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Eroare la comunicarea cu serverul");
                await Shell.Current.DisplayAlert("Eroare",
                    "Nu s-a putut stabili conexiunea cu serverul. Verificați conexiunea la internet.", "OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eroare la salvarea datelor");
                await Shell.Current.DisplayAlert("Eroare",
                    "Nu s-au putut salva modificările. Încercați din nou.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}