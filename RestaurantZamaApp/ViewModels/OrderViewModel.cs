using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
//using RestaurantZamaApp.Models;
using RestaurantZamaShared.Models;
using RestaurantZamaApp.Services;
using Microsoft.Extensions.Logging;

namespace RestaurantZamaApp.ViewModels
{
    public partial class OrderViewModel : ObservableObject
    {
        private readonly OrderService _orderService;
        private readonly ILogger<OrderViewModel> _logger;

        [ObservableProperty]
        private ObservableCollection<Order> _orders = new();

        [ObservableProperty]
        private Order _currentOrder;

        [ObservableProperty]
        private bool _isBusy;

        public OrderViewModel(OrderService orderService, ILogger<OrderViewModel> logger)
        {
            _orderService = orderService;
            _logger = logger;
            _currentOrder = new Order { OrderItems = new List<OrderItem>() };
        }

        [RelayCommand]
        private async Task LoadOrdersAsync()
        {
            if (_isBusy) return;
            try
            {
                _isBusy = true;
                var userId = Preferences.Get("UserId", 0);
                var orders = await _orderService.GetUserOrdersAsync(userId);

                _orders.Clear();
                foreach (var order in orders)
                {
                    _orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading orders");
                await Shell.Current.DisplayAlert("Eroare", "Nu s-au putut încărca comenzile", "OK");
            }
            finally
            {
                _isBusy = false;
            }
        }

        [RelayCommand]
        private async Task CreateOrderAsync()
        {
            if (_isBusy) return;
            try
            {
                _isBusy = true;
                _currentOrder.UserId = Preferences.Get("UserId", 0);
                _currentOrder.OrderDate = DateTime.UtcNow;
                _currentOrder.Status = "Pending";

                var newOrder = await _orderService.CreateOrderAsync(_currentOrder);
                _orders.Add(newOrder);

                await Shell.Current.DisplayAlert("Succes", "Comandă creată cu succes", "OK");

                // Resetare comandă curentă
                _currentOrder = new Order { OrderItems = new List<OrderItem>() };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                await Shell.Current.DisplayAlert("Eroare", "Nu s-a putut crea comanda", "OK");
            }
            finally
            {
                _isBusy = false;
            }
        }

        [RelayCommand]
        private void AddOrderItem(RestaurantZamaShared.Models.MenuItem menuItem)
        {
            var existingItem = _currentOrder.OrderItems.FirstOrDefault(oi => oi.MenuItemId == menuItem.Id);

            if (existingItem != null)
            {
                existingItem.Quantity++;
                existingItem.TotalPrice = existingItem.Quantity * existingItem.UnitPrice;
            }
            else
            {
                _currentOrder.OrderItems.Add(new OrderItem
                {
                    MenuItemId = menuItem.Id,
                    Quantity = 1,
                    UnitPrice = menuItem.Price,
                    TotalPrice = menuItem.Price
                });
            }

            _currentOrder.TotalAmount = _currentOrder.OrderItems.Sum(oi => oi.TotalPrice);
        }

        [RelayCommand]
        private async Task UpdateOrderStatusAsync(string newStatus)
        {
            if (_isBusy || _currentOrder.Id == 0) return;
            try
            {
                _isBusy = true;
                await _orderService.UpdateOrderStatusAsync(_currentOrder.Id, newStatus);
                _currentOrder.Status = newStatus;

                await Shell.Current.DisplayAlert("Succes", $"Status comandă actualizat la {newStatus}", "OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status");
                await Shell.Current.DisplayAlert("Eroare", "Nu s-a putut actualiza statusul comenzii", "OK");
            }
            finally
            {
                _isBusy = false;
            }
        }
    }
}
