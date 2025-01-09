using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using RestaurantZamaShared.Models;

namespace RestaurantZamaApp.Services
{
    public class OrderService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderService> _logger;
        private const string BaseUrl = "api/orders";

        public OrderService(HttpClient httpClient, ILogger<OrderService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // Metodele CRUD
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<Order>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                throw;
            }
        }

        public async Task<List<Order>> GetUserOrdersAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/user/{userId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<Order>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting orders for user {userId}");
                throw;
            }
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/{orderId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Order>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting order with ID {orderId}");
                throw;
            }
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, order);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Order>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                throw;
            }
        }

        public async Task<Order> UpdateOrderAsync(int orderId, Order order)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{orderId}", order);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Order>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order with ID {orderId}");
                throw;
            }
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{orderId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order with ID {orderId}");
                throw;
            }
        }

        // Metodă pentru schimbarea statusului comenzii
        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{orderId}/status", new { status = newStatus });
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order status for order {orderId}");
                throw;
            }
        }
    }
}