using RestaurantZamaApp.ViewModels;

namespace RestaurantZamaApp.Views
{
    public partial class MenuPage : ContentPage
    {
        private readonly MenuViewModel _viewModel;

        public MenuPage(MenuViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel != null)
            {
                _viewModel.LoadMenuItemsCommand.Execute(null);
            }
        }
    }
}