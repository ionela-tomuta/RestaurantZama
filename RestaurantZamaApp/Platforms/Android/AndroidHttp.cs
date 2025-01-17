﻿using System.Net.Security;
using Xamarin.Android.Net;

namespace RestaurantZamaApp
{
    public class AndroidHttp: IPlatformHttp
    {
        public HttpMessageHandler GetHttpMessageHandler() => new AndroidMessageHandler
        {
            ServerCertificateCustomValidationCallback = (httpRequestMessage, certificate, chain, sslPolicyErrors) =>
                certificate?.Issuer == "CN=localhost" || sslPolicyErrors == SslPolicyErrors.None
        };
    }
}