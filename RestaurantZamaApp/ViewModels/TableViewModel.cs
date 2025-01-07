using RestaurantZamaApp.Models;
using RestaurantZamaApp.Services;

namespace RestaurantZamaApp.ViewModels
{
    public class TableViewModel
    {
        public int Id { get; private set; }
        public int Number { get; private set; }
        public int Capacity { get; private set; }
        public string Location { get; private set; }

        public string DisplayName => $"Table {Number} ({Capacity} seats - {Location})";

        public TableViewModel(RestaurantZamaApp.Models.Table table)
        {
            Id = table.Id;
            Number = table.Number;
            Capacity = table.Capacity;
            Location = table.Location;
        }
    }
}