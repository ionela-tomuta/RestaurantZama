using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using RestaurantZamaApp.Models;
using RestaurantZamaApp.Services;

namespace RestaurantZamaApp.ViewModels
{
    public partial class MenuViewModel : ObservableObject
    {
        private readonly IMenuService _menuService;

        [ObservableProperty]
        private ObservableCollection<RestaurantZamaApp.Models.MenuItem> _menuItems = new();

        [ObservableProperty]
        private bool _isBusy;

        public MenuViewModel(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [RelayCommand]
        private async Task LoadMenuItems()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var items = await _menuService.GetMenuItemsAsync();
                MenuItems.Clear();
                foreach (var item in items)
                {
                    MenuItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Could not load menu items", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}