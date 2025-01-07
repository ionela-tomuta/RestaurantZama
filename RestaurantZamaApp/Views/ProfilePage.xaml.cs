using RestaurantZamaApp.ViewModels;

namespace RestaurantZamaApp.Views
{
    public partial class ProfilePage : ContentPage
    {
        private readonly ProfilePageViewModel _viewModel;

        public ProfilePage(ProfilePageViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel != null)
            {
                await _viewModel.LoadUserData();
            }
        }
    }
}