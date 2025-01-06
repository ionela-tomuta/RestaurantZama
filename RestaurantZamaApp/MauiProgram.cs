using RestaurantZamaApp.Models;
using RestaurantZamaApp.Services;
using RestaurantZamaApp.Views;
using RestaurantZamaApp.ViewModels;
using RestaurantZamaApp;
using Microsoft.Extensions.Logging;

namespace RestaurantZamaApp
{
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
            }).ConfigureHttpMessageHandlerBuilder(configBuilder =>
            {
                var platformMessageHandler = configBuilder.Services.GetRequiredService<IPlatformHttp>();
                configBuilder.PrimaryHandler = platformMessageHandler.GetHttpMessageHandler();
            });


            builder.Services.AddSingleton<ClientService>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<WeatherForecastPage>();
            builder.Services.AddSingleton<WeatherForecastViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}