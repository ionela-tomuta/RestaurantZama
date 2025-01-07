using RestaurantZamaApp.Views;
using RestaurantZamaApp.ViewModels;
using RestaurantZamaApp.Services;
using RestaurantZamaApp.Data;
using RestaurantZamaApp.Models;

namespace RestaurantZamaApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(ReservationsPage), typeof(ReservationsPage));
            Routing.RegisterRoute(nameof(OrderPage), typeof(OrderPage));
            Routing.RegisterRoute(nameof(MenuPage), typeof(MenuPage));
        }
    }
}
