using RestaurantZamaApp.ViewModels;

namespace RestaurantZamaApp.Views;

public partial class WeatherForecastPage : ContentPage
{
    public WeatherForecastPage(WeatherForecastViewModel weatherForecastViewModel)
    {
        InitializeComponent();
        BindingContext = weatherForecastViewModel;
    }
}