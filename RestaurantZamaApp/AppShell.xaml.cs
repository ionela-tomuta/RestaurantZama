using RestaurantZamaApp.Views;

namespace RestaurantZamaApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(WeatherForecastPage), typeof(WeatherForecastPage));

        }
    }
}
