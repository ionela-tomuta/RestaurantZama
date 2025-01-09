using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantZamaShared.Models
{
    public class RegisterModel
    {
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

    }
}
