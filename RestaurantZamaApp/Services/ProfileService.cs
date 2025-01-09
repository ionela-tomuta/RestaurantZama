using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using RestaurantZamaShared.Models;

namespace RestaurantZamaApp.Services
{
    public interface IProfileService
    {
        Task<User> CreateProfileAsync(User profile);
        Task<User> GetProfileByIdAsync(int id);
        Task<List<User>> GetAllProfilesAsync();
        Task<User> UpdateProfileAsync(int id, User profile);
        Task<bool> DeleteProfileAsync(int id);
    }

    public class ProfileService : IProfileService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProfileService> _logger;
        private const string BaseUrl = "api/users"; // Changed from api/profiles to api/users

        public ProfileService(HttpClient httpClient, ILogger<ProfileService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        // CREATE 
        public async Task<User> CreateProfileAsync(User profile)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, profile);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating profile");
                throw;
            }
        }

        // READ (By ID)
        public async Task<User> GetProfileByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting profile with ID {id}");
                throw;
            }
        }

        // READ (All)
        public async Task<List<User>> GetAllProfilesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<User>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all profiles");
                throw;
            }
        }

        // UPDATE

        public async Task<User> UpdateProfileAsync(int id, User profile)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", profile);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new KeyNotFoundException($"Profilul cu ID {id} nu a fost găsit.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Server returned {response.StatusCode}: {errorMessage}");
                    throw new HttpRequestException($"Eroare server: {response.StatusCode}");
                }

                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Eroare la actualizarea profilului cu ID {id}");
                throw;
            }
        }


        // DELETE
        public async Task<bool> DeleteProfileAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting profile with ID {id}");
                throw;
            }
        }
    }
}