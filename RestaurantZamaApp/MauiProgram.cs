using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Http;
using RestaurantZamaShared.Models;
using RestaurantZamaApp.Services;
using RestaurantZamaApp.Views;
using RestaurantZamaApp.ViewModels;
using Microsoft.Extensions.Configuration;
using RestaurantZamaApp.Converters;

namespace RestaurantZamaApp;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.UseMauiApp<App>()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IPlatformHttp>(sp => {
#if ANDROID
            return new AndroidHttp();
#else
           return null!;
#endif
        });

        builder.Services.AddHttpClient("custom-httpclient", client => {
            var baseAddress = DeviceInfo.Platform == DevicePlatform.Android
                ? "https://10.0.2.2:7129"
                : "https://localhost:7129";
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }).ConfigurePrimaryHttpMessageHandler(sp => {
            var platformMessageHandler = sp.GetRequiredService<IPlatformHttp>();
            return platformMessageHandler.GetHttpMessageHandler();
        });

        // Services
        //builder.Services.AddTransient<IProfileService, ProfileService>();
        builder.Services.AddHttpClient<IProfileService, ProfileService>(client => {
            var baseAddress = DeviceInfo.Platform == DevicePlatform.Android
                ? "https://10.0.2.2:7129"
                : "https://localhost:7129";
            client.BaseAddress = new Uri(baseAddress);
        });

        builder.Services.AddSingleton<IMenuService, MenuService>();
        builder.Services.AddSingleton<OrderService>();
        builder.Services.AddSingleton<IReservationService, ReservationService>();
        builder.Services.AddSingleton<ClientService>();

        // Pages & ViewModels
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<ProfilePage>();
        builder.Services.AddSingleton<ProfilePageViewModel>();
        builder.Services.AddTransient<MenuPage>();
        builder.Services.AddSingleton<ITablesService, TablesService>();
        builder.Services.AddTransient<MenuViewModel>();
        builder.Services.AddTransient<OrderPage>();
        builder.Services.AddTransient<OrderViewModel>();
        builder.Services.AddTransient<ReservationsPage>();
        builder.Services.AddTransient<ReservationsPageViewModel>();

        builder.Services.AddSingleton<IValueConverter, InverseBoolConverter>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}