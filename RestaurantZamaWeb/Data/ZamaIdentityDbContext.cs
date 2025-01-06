using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ZamaRestaurantWeb.Data
{
    public class ZamaIdentityDbContext : IdentityDbContext
    {
        public ZamaIdentityDbContext(DbContextOptions<ZamaIdentityDbContext> options) : base(options)
        {
        }
    }
}
