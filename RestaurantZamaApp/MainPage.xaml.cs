using RestaurantZamaApp.ViewModels;

namespace RestaurantZamaApp
{
    public partial class MainPage : ContentPage
    {
            public MainPage(MainPageViewModel MainPageViewModel)
            {
                InitializeComponent();
                BindingContext = MainPageViewModel;
            }

        }
    }

