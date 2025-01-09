using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using RestaurantZamaShared.Models;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using System.IO;
using System.Linq;

namespace RestaurantZamaApp.Services
{
    public interface IMenuService
    {
        Task<List<RestaurantZamaShared.Models.MenuItem>> GetMenuItemsAsync();
    }

    public class MenuService : IMenuService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MenuService> _logger;
        private const string BaseUrl = "api/menu";

        public MenuService(HttpClient httpClient, ILogger<MenuService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OpenMenuPdf()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/pdf");
                response.EnsureSuccessStatusCode();
                var pdfBytes = await response.Content.ReadAsByteArrayAsync();

                var tempFile = Path.Combine(FileSystem.CacheDirectory, "menu.pdf");
                await File.WriteAllBytesAsync(tempFile, pdfBytes);

                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(tempFile)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening menu PDF");
                throw;
            }
        }

        public async Task<List<string>> GetMenuCategories()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/categories");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<string>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting menu categories");
                throw;
            }
        }

        public async Task<List<string>> GetDailySpecials()
        {
            try
            {
                var specialItems = await GetDailySpecialsAsync();
                return specialItems.Select(item => item.Name).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily specials");
                throw;
            }
        }

        public async Task<List<RestaurantZamaShared.Models.MenuItem>> GetMenuItemsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/items");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<RestaurantZamaShared.Models.MenuItem>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting menu items");
                throw;
            }
        }

        public async Task<RestaurantZamaShared.Models.MenuItem> CreateMenuItemAsync(RestaurantZamaShared.Models.MenuItem menuItem)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/items", menuItem);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<RestaurantZamaShared.Models.MenuItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating menu item");
                throw;
            }
        }

        public async Task<RestaurantZamaShared.Models.MenuItem> GetMenuItemByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/items/{id}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<RestaurantZamaShared.Models.MenuItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting menu item with ID {id}");
                throw;
            }
        }

        public async Task<RestaurantZamaShared.Models.MenuItem> UpdateMenuItemAsync(int id, RestaurantZamaShared.Models.MenuItem menuItem)
        {
            try
            {
                if (id != menuItem.Id)
                {
                    throw new ArgumentException("ID mismatch between path and body");
                }

                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/items/{id}", menuItem);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<RestaurantZamaShared.Models.MenuItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating menu item with ID {id}");
                throw;
            }
        }

        public async Task<bool> DeleteMenuItemAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/items/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting menu item with ID {id}");
                throw;
            }
        }

        public async Task<List<RestaurantZamaShared.Models.MenuItem>> GetDailySpecialsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/daily-specials");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<RestaurantZamaShared.Models.MenuItem>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily specials");
                throw;
            }
        }

        public async Task<List<RestaurantZamaShared.Models.MenuItem>> GetMenuItemsByCategoryAsync(string category)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/items/category/{Uri.EscapeDataString(category)}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<RestaurantZamaShared.Models.MenuItem>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting menu items for category {category}");
                throw;
            }
        }

        public async Task<bool> UpdateMenuItemAvailabilityAsync(int id, bool isAvailable)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/items/{id}/availability",
                    new { isAvailable });
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating availability for menu item {id}");
                throw;
            }
        }

        public async Task<bool> SetSpecialOfTheDayAsync(int id, decimal? specialPrice)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/items/{id}/special",
                    new { specialPrice });
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting special of the day for menu item {id}");
                throw;
            }
        }
    }
}