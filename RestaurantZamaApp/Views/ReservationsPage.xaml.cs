using Microsoft.Extensions.Logging;
using RestaurantZamaApp.ViewModels;

namespace RestaurantZamaApp.Views
{
    public partial class ReservationsPage : ContentPage
    {
        private readonly ReservationsPageViewModel _viewModel;
        private readonly ILogger<ReservationsPage> _logger;

        public ReservationsPage(ReservationsPageViewModel viewModel, ILogger<ReservationsPage> logger)
        {
            InitializeComponent();
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            BindingContext = _viewModel;
        }

        }
    }