using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

public class ReservationsPageViewModel : INotifyPropertyChanged
{
    private readonly IReservationService _reservationService;

    public ReservationsPageViewModel(IReservationService reservationService)
    {
        _reservationService = reservationService;
        LoadTablesCommand = new RelayCommand(async () => await LoadTables());
        LoadAvailableData(); // Asigură-te că încărcăm datele la crearea ViewModel-ului
    }

    public ICommand LoadTablesCommand { get; }

    private ObservableCollection<Table> _availableTables;
    public ObservableCollection<Table> AvailableTables
    {
        get => _availableTables;
        set
        {
            _availableTables = value;
            OnPropertyChanged(nameof(AvailableTables));
        }
    }

    private ObservableCollection<string> _availableDurations;
    public ObservableCollection<string> AvailableDurations
    {
        get => _availableDurations;
        set
        {
            _availableDurations = value;
            OnPropertyChanged(nameof(AvailableDurations));
        }
    }

    private async Task LoadTables()
    {
        var tables = await _reservationService.GetAvailableTablesAsync();
        AvailableTables = new ObservableCollection<Table>(tables);
    }

    private async Task LoadAvailableData()
    {
        AvailableTables = new ObservableCollection<Table>(await _reservationService.GetAvailableTablesAsync());
        AvailableDurations = new ObservableCollection<string> { "30 min", "1 h", "2 h" }; // Exemplu
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class Table
{
    public string Name { get; set; }
    public int Capacity { get; set; }
}

public interface IReservationService
{
    Task<IEnumerable<Table>> GetAvailableTablesAsync();
}
