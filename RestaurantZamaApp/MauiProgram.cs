using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Http;
using RestaurantZamaApp.Models;
using RestaurantZamaApp.Services;
using RestaurantZamaApp.Views;
using RestaurantZamaApp.ViewModels;
using RestaurantZamaApp.Data;
using Microsoft.Extensions.Configuration;
using RestaurantZamaApp.Converters; 

namespace RestaurantZamaApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Platform-specific HTTP configuration
        builder.Services.AddSingleton<IPlatformHttp>(sp =>
        {
#if ANDROID
            return new AndroidHttp();
#else
            return null!;
#endif
        });

        builder.Services.AddHttpClient("custom-httpclient", httpClient =>
        {
            var baseAddress = DeviceInfo.Platform == DevicePlatform.Android ?
                "https://10.0.2.2:7129" : "https://localhost:7129";
            httpClient.BaseAddress = new Uri(baseAddress);
        }).ConfigurePrimaryHttpMessageHandler(sp =>
        {
            var platformMessageHandler = sp.GetRequiredService<IPlatformHttp>();
            return platformMessageHandler.GetHttpMessageHandler();
        });

        // Configurarea DbContext pentru Entity Framework Core
        builder.Services.AddDbContext<ZamaDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        // Singleton Services
        builder.Services.AddSingleton<IProfileService, ProfileService>();
        builder.Services.AddSingleton<IMenuService, MenuService>();
        builder.Services.AddSingleton<OrderService>();
        builder.Services.AddSingleton<ReservationService>();
        builder.Services.AddSingleton<ClientService>();

        // Main Pages and ViewModels (Singleton)
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<ProfilePage>();
        builder.Services.AddSingleton<ProfilePageViewModel>();

        // Transient Pages and ViewModels
        builder.Services.AddTransient<MenuPage>();
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