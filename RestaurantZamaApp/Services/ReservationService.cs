using Microsoft.Extensions.Logging;
using RestaurantZamaApp.Models;
using System.Net.Http.Json;

namespace RestaurantZamaApp.Services
{
    public interface IReservationService
    {
        Task<List<Table>> GetAvailableTablesAsync();
        Task<List<Reservation>> GetUserReservationsAsync(int userId);
        Task<Reservation> CreateReservationAsync(Reservation reservation);
        Task<bool> CancelReservationAsync(int reservationId);
        Task<bool> UpdateReservationStatusAsync(int reservationId, string status);
    }
        public class ReservationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReservationService> _logger;
        private const string BaseUrl = "api/reservations";

        public ReservationService(HttpClient httpClient, ILogger<ReservationService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Metodele existente rãmân neschimbate
        public async Task<List<Reservation>> GetUserReservationsAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/user/{userId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<Reservation>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user reservations");
                throw;
            }
        }

        public async Task<List<Table>> GetAvailableTablesAsync(DateTime date, TimeSpan time, TimeSpan duration)
        {
            try
            {
                var query = $"{BaseUrl}/available-tables?" +
                           $"date={date:yyyy-MM-dd}&" +
                           $"time={time:hh\\:mm}&" +
                           $"duration={duration:hh\\:mm}";

                var response = await _httpClient.GetAsync(query);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<Table>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available tables");
                throw;
            }
        }

        public async Task<bool> IsTableAvailableAsync(int tableId, DateTime date, TimeSpan time, TimeSpan duration)
        {
            try
            {
                var query = $"{BaseUrl}/check-availability?" +
                           $"tableId={tableId}&" +
                           $"date={date:yyyy-MM-dd}&" +
                           $"time={time:hh\\:mm}&" +
                           $"duration={duration:hh\\:mm}";

                var response = await _httpClient.GetAsync(query);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking table availability");
                throw;
            }
        }

        public async Task<Reservation> CreateReservationAsync(Reservation reservation)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, reservation);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Reservation>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reservation");
                throw;
            }
        }

        public async Task<bool> CancelReservationAsync(int reservationId)
        {
            try
            {
                var response = await _httpClient.PutAsync($"{BaseUrl}/{reservationId}/cancel", null);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling reservation");
                throw;
            }
        }

        public async Task<bool> UpdateReservationStatusAsync(int reservationId, string status)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(
                    $"{BaseUrl}/{reservationId}/status",
                    new { status });

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reservation status");
                throw;
            }
        }

        // NOI METODE CRUD

        // Preluare toate rezervãrile
        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<Reservation>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all reservations");
                throw;
            }
        }

        // Preluare rezervare dupã ID
        public async Task<Reservation> GetReservationByIdAsync(int reservationId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/{reservationId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Reservation>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting reservation with ID {reservationId}");
                throw;
            }
        }

        // Actualizare rezervare
        public async Task<Reservation> UpdateReservationAsync(int reservationId, Reservation reservation)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{reservationId}", reservation);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Reservation>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating reservation with ID {reservationId}");
                throw;
            }
        }

        // ?tergere rezervare
        public async Task<bool> DeleteReservationAsync(int reservationId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{reservationId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting reservation with ID {reservationId}");
                throw;
            }
        }
    }
}