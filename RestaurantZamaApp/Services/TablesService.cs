using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RestaurantZamaApp.ViewModels;

namespace RestaurantZamaApp.Services
{
    public interface ITablesService
    {
        Task<List<TableViewModel>> GetAvailableTablesAsync(
            DateTime? date = null,
            TimeSpan? time = null,
            TimeSpan? duration = null,
            int? numberOfGuests = null);
    }
}
namespace RestaurantZamaApp.Services
{
    public class TablesService : ITablesService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public TablesService(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl.TrimEnd('/') + "/api/reservations/";
        }

        public async Task<List<TableViewModel>> GetAvailableTablesAsync(
            DateTime? date = null,
            TimeSpan? time = null,
            TimeSpan? duration = null,
            int? numberOfGuests = null)
        {
            try
            {
                Debug.WriteLine($"Getting available tables with parameters: Date={date}, Time={time}, Duration={duration}, Guests={numberOfGuests}");

                var query = new List<string>();
                if (date.HasValue)
                    query.Add($"date={date.Value:yyyy-MM-dd}");
                if (time.HasValue)
                    query.Add($"time={time.Value:hh\\:mm}");
                if (duration.HasValue)
                    query.Add($"duration={duration.Value:hh\\:mm}");
                if (numberOfGuests.HasValue)
                    query.Add($"numberOfGuests={numberOfGuests.Value}");

                var url = $"{_baseUrl}available-tables";
                if (query.Any())
                    url += "?" + string.Join("&", query);

                Debug.WriteLine($"Request URL: {url}");

                var response = await _httpClient.GetAsync(url);
                Debug.WriteLine($"Response status code: {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Response content: {content}");

                if (response.IsSuccessStatusCode)
                {
                    var tables = JsonSerializer.Deserialize<List<RestaurantZamaShared.Models.Table>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    Debug.WriteLine($"Deserialized {tables?.Count ?? 0} tables");

                    return tables?.Select(t => new TableViewModel(t)).ToList() ?? new List<TableViewModel>();
                }
                else
                {
                    Debug.WriteLine($"Error response: {content}");
                    throw new Exception($"Failed to get tables: {response.StatusCode} - {content}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in GetAvailableTablesAsync: {ex}");
                throw;
            }
        }
    }
}
