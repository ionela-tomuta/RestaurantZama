using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using RestaurantZamaApp.Models;

namespace RestaurantZamaApp.Services
{
    public interface IProfileService
    {
        // Create (Deși puțin ciudat pentru profil, îl păstrăm pentru consecvență)
        Task<User> CreateProfileAsync(User profile);

        // Read
        Task<User> GetProfileByIdAsync(int id);
        Task<List<User>> GetAllProfilesAsync();

        // Update
        Task<User> UpdateProfileAsync(int id, User profile);

        // Delete
        Task<bool> DeleteProfileAsync(int id);
    }

    public class ProfileService : IProfileService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProfileService> _logger;
        private const string BaseUrl = "api/profiles";

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
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating profile with ID {id}");
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