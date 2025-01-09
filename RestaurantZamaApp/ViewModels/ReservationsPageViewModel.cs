using System.ComponentModel;
using System.Windows.Input;
using RestaurantZamaShared.Models;
using RestaurantZamaApp.Services;
using CommunityToolkit.Mvvm.Input;

namespace RestaurantZamaApp.ViewModels
{
    public class ReservationsPageViewModel : INotifyPropertyChanged
    {
        private readonly IReservationService _reservationService;

        private DateTime _reservationDate = DateTime.Today;
        public DateTime ReservationDate
        {
            get => _reservationDate;
            set
            {
                _reservationDate = value;
                OnPropertyChanged(nameof(ReservationDate));
            }
        }

        private TimeSpan _reservationTime = DateTime.Now.TimeOfDay;
        public TimeSpan ReservationTime
        {
            get => _reservationTime;
            set
            {
                _reservationTime = value;
                OnPropertyChanged(nameof(ReservationTime));
            }
        }

        private TimeSpan _duration = TimeSpan.FromHours(1);
        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }

        private int _numberOfGuests = 2;
        public int NumberOfGuests
        {
            get => _numberOfGuests;
            set
            {
                _numberOfGuests = value;
                OnPropertyChanged(nameof(NumberOfGuests));
            }
        }

        private string _contactPhone;
        public string ContactPhone
        {
            get => _contactPhone;
            set
            {
                _contactPhone = value;
                OnPropertyChanged(nameof(ContactPhone));
            }
        }

        private string _specialRequests;
        public string SpecialRequests
        {
            get => _specialRequests;
            set
            {
                _specialRequests = value;
                OnPropertyChanged(nameof(SpecialRequests));
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public DateTime MinimumDate => DateTime.Today;

        public ReservationsPageViewModel(IReservationService reservationService)
        {
            _reservationService = reservationService;
            SubmitReservationCommand = new RelayCommand(async () => await SubmitReservation(),
                () => !string.IsNullOrWhiteSpace(ContactPhone));
        }

        public ICommand SubmitReservationCommand { get; }

        private async Task SubmitReservation()
        {
            try
            {
                IsBusy = true;

                // Validări
                if (string.IsNullOrWhiteSpace(ContactPhone))
                {
                    await Shell.Current.DisplayAlert("Eroare", "Introduceți un număr de telefon", "OK");
                    return;
                }

                if (NumberOfGuests <= 0 || NumberOfGuests > 20)
                {
                    await Shell.Current.DisplayAlert("Eroare", "Numărul de persoane trebuie să fie între 1 și 20", "OK");
                    return;
                }

                if (Duration < TimeSpan.FromMinutes(30) || Duration > TimeSpan.FromHours(4))
                {
                    await Shell.Current.DisplayAlert("Eroare", "Durata trebuie să fie între 30 minute și 4 ore", "OK");
                    return;
                }

                // Creare rezervare
                var newReservation = new Reservation
                {
                    UserId = Preferences.Get("UserId", 0),
                    ReservationDate = ReservationDate,
                    ReservationTime = ReservationTime,
                    NumberOfGuests = NumberOfGuests,
                    ContactPhone = ContactPhone,
                    SpecialRequests = SpecialRequests,
                    Status = "Pending"
                };

                var createdReservation = await _reservationService.CreateReservationAsync(newReservation);

                await Shell.Current.DisplayAlert("Succes",
                    "Rezervarea a fost creată cu succes!", "OK");

                // Resetare câmpuri după creare
                ResetReservationFields();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Eroare",
                    $"Nu s-a putut crea rezervarea: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ResetReservationFields()
        {
            ContactPhone = string.Empty;
            SpecialRequests = string.Empty;
            Name = string.Empty;
            NumberOfGuests = 2;
            ReservationDate = DateTime.Today;
            ReservationTime = DateTime.Now.TimeOfDay;
            Duration = TimeSpan.FromHours(1);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}