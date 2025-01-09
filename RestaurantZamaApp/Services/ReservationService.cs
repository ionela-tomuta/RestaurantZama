using Microsoft.Extensions.Logging;
using RestaurantZamaShared.Models;
using System.Net.Http.Json;

namespace RestaurantZamaApp.Services
{
    public interface IReservationService
    {
        Task<List<Table>> GetAvailableTablesAsync(DateTime date, TimeSpan time, TimeSpan duration, int numberOfGuests);
        Task<List<Reservation>> GetUserReservationsAsync(int userId);
        Task<Reservation> CreateReservationAsync(Reservation reservation);
        Task<bool> CancelReservationAsync(int reservationId);
        Task<bool> UpdateReservationStatusAsync(int reservationId, string status);
        Task<bool> IsTableAvailableAsync(int tableId, DateTime date, TimeSpan time, TimeSpan duration);
        Task<List<Reservation>> GetAllReservationsAsync();
        Task<Reservation> GetReservationByIdAsync(int reservationId);
        Task<Reservation> UpdateReservationAsync(int reservationId, Reservation reservation);
        Task<bool> DeleteReservationAsync(int reservationId);
    }

    public class ReservationService : IReservationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReservationService> _logger;
        private const string BaseUrl = "api/reservations";

        public ReservationService(IHttpClientFactory httpClientFactory, ILogger<ReservationService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("custom-httpclient");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private async Task<T> HandleHttpRequestAsync<T>(Func<Task<HttpResponseMessage>> httpRequest, string errorMessage)
        {
            try
            {
                var response = await httpRequest();
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorMessage);
                throw new InvalidOperationException($"{errorMessage}: {ex.Message}", ex);
            }
        }

        public async Task<List<Table>> GetAvailableTablesAsync(DateTime date, TimeSpan time, TimeSpan duration, int numberOfGuests)
        {
            var query = $"{BaseUrl}/available-tables?" +
                        $"date={date:yyyy-MM-dd}&" +
                        $"time={time:hh\\:mm}&" +
                        $"duration={duration:hh\\:mm}&" +
                        $"numberOfGuests={numberOfGuests}";

            return await HandleHttpRequestAsync<List<Table>>(
                () => _httpClient.GetAsync(query),
                "Error getting available tables"
            );
        }

        public async Task<List<Reservation>> GetUserReservationsAsync(int userId)
        {
            var url = $"{BaseUrl}/user/{userId}";
            return await HandleHttpRequestAsync<List<Reservation>>(
                () => _httpClient.GetAsync(url),
                "Error getting user reservations"
            );
        }

        public async Task<Reservation> CreateReservationAsync(Reservation reservation)
        {
            return await HandleHttpRequestAsync<Reservation>(
                () => _httpClient.PostAsJsonAsync(BaseUrl, reservation),
                "Error creating reservation"
            );
        }

        public async Task<bool> CancelReservationAsync(int reservationId)
        {
            var url = $"{BaseUrl}/{reservationId}/cancel";
            return await HandleHttpRequestAsync<bool>(
                () => _httpClient.PutAsync(url, null),
                $"Error cancelling reservation with ID {reservationId}"
            );
        }

        public async Task<bool> UpdateReservationStatusAsync(int reservationId, string status)
        {
            var url = $"{BaseUrl}/{reservationId}/status";
            return await HandleHttpRequestAsync<bool>(
                () => _httpClient.PutAsJsonAsync(url, new { status }),
                $"Error updating status for reservation with ID {reservationId}"
            );
        }

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            return await HandleHttpRequestAsync<List<Reservation>>(
                () => _httpClient.GetAsync(BaseUrl),
                "Error getting all reservations"
            );
        }

        public async Task<Reservation> GetReservationByIdAsync(int reservationId)
        {
            var url = $"{BaseUrl}/{reservationId}";
            return await HandleHttpRequestAsync<Reservation>(
                () => _httpClient.GetAsync(url),
                $"Error getting reservation with ID {reservationId}"
            );
        }

        public async Task<Reservation> UpdateReservationAsync(int reservationId, Reservation reservation)
        {
            var url = $"{BaseUrl}/{reservationId}";
            return await HandleHttpRequestAsync<Reservation>(
                () => _httpClient.PutAsJsonAsync(url, reservation),
                $"Error updating reservation with ID {reservationId}"
            );
        }

        public async Task<bool> DeleteReservationAsync(int reservationId)
        {
            var url = $"{BaseUrl}/{reservationId}";
            return await HandleHttpRequestAsync<bool>(
                () => _httpClient.DeleteAsync(url),
                $"Error deleting reservation with ID {reservationId}"
            );
        }

        public async Task<bool> IsTableAvailableAsync(int tableId, DateTime date, TimeSpan time, TimeSpan duration)
        {
            var query = $"{BaseUrl}/check-availability?" +
                        $"tableId={tableId}&" +
                        $"date={date:yyyy-MM-dd}&" +
                        $"time={time:hh\\:mm}&" +
                        $"duration={duration:hh\\:mm}";

            return await HandleHttpRequestAsync<bool>(
                () => _httpClient.GetAsync(query),
                $"Error checking availability for table with ID {tableId}"
            );
        }
    }
}
