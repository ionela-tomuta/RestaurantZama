using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantZamaShared.Models;
using ZamaRestaurantWeb.Data;

namespace RestaurantZamaWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly ZamaDbContext _context;
        private readonly ILogger<MenuController> _logger;
        private readonly IWebHostEnvironment _environment;

        public MenuController(
            ZamaDbContext context,
            ILogger<MenuController> logger,
            IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        // GET: api/menu/items
        [HttpGet("items")]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetMenuItems()
        {
            try
            {
                var menuItems = await _context.MenuItems.ToListAsync();
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving menu items");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/menu/items/{id}
        [HttpGet("items/{id}")]
        public async Task<ActionResult<MenuItem>> GetMenuItem(int id)
        {
            try
            {
                var menuItem = await _context.MenuItems.FindAsync(id);

                if (menuItem == null)
                {
                    return NotFound($"Menu item with ID {id} not found");
                }

                return Ok(menuItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving menu item with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/menu/categories
        [HttpGet("categories")]
        public async Task<ActionResult<List<string>>> GetMenuCategories()
        {
            try
            {
                // This is a placeholder. In a real app, you might have a separate Categories table or 
                // derive categories from menu items
                var categories = new List<string>
                {
                    "Appetizers",
                    "Main Courses",
                    "Desserts",
                    "Drinks"
                };
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving menu categories");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/menu/items/category/{category}
        [HttpGet("items/category/{category}")]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetMenuItemsByCategory(string category)
        {
            try
            {
                // Placeholder implementation. In a real app, you'd have a Category property in MenuItem
                var menuItems = await _context.MenuItems
                    .Where(m => m.Name.Contains(category, StringComparison.OrdinalIgnoreCase))
                    .ToListAsync();

                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving menu items for category {category}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/menu/daily-specials
        [HttpGet("daily-specials")]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetDailySpecials()
        {
            try
            {
                // Placeholder implementation. In a real app, you'd have a way to mark daily specials
                var dailySpecials = await _context.MenuItems
                    .Take(3) // Example: first 3 items as specials
                    .ToListAsync();

                return Ok(dailySpecials);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving daily specials");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/menu/items
        [HttpPost("items")]
        public async Task<ActionResult<MenuItem>> CreateMenuItem(MenuItem menuItem)
        {
            try
            {
                if (menuItem == null)
                {
                    return BadRequest("Invalid menu item");
                }

                _context.MenuItems.Add(menuItem);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.Id }, menuItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating menu item");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/menu/items/{id}
        [HttpPut("items/{id}")]
        public async Task<ActionResult<MenuItem>> UpdateMenuItem(int id, MenuItem menuItem)
        {
            try
            {
                if (id != menuItem.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var existingItem = await _context.MenuItems.FindAsync(id);
                if (existingItem == null)
                {
                    return NotFound($"Menu item with ID {id} not found");
                }

                // Update properties
                existingItem.Name = menuItem.Name;
                existingItem.Price = menuItem.Price;

                await _context.SaveChangesAsync();

                return Ok(existingItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating menu item with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/menu/items/{id}/availability
        [HttpPut("items/{id}/availability")]
        public async Task<IActionResult> UpdateMenuItemAvailability(int id, [FromBody] bool isAvailable)
        {
            try
            {
                var menuItem = await _context.MenuItems.FindAsync(id);
                if (menuItem == null)
                {
                    return NotFound($"Menu item with ID {id} not found");
                }

                // In a real app, you'd add an IsAvailable property to MenuItem
                // For now, this is a placeholder
                _logger.LogInformation($"Updated availability for item {id}: {isAvailable}");

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating availability for menu item {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/menu/items/{id}/special
        [HttpPut("items/{id}/special")]
        public async Task<IActionResult> SetSpecialOfTheDay(int id, [FromBody] decimal? specialPrice)
        {
            try
            {
                var menuItem = await _context.MenuItems.FindAsync(id);
                if (menuItem == null)
                {
                    return NotFound($"Menu item with ID {id} not found");
                }

                // In a real app, you'd add logic to set daily specials
                if (specialPrice.HasValue)
                {
                    menuItem.Price = specialPrice.Value;
                }

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting special of the day for menu item {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/menu/pdf
        [HttpGet("pdf")]
        public async Task<IActionResult> GetMenuPdf()
        {
            try
            {
                var path = Path.Combine(_environment.WebRootPath, "menu.pdf");

                if (!System.IO.File.Exists(path))
                {
                    return NotFound("Menu PDF not found");
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                return File(memory, "application/pdf", "menu.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving menu PDF");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/menu/items/{id}
        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            try
            {
                var menuItem = await _context.MenuItems.FindAsync(id);
                if (menuItem == null)
                {
                    return NotFound($"Menu item with ID {id} not found");
                }

                _context.MenuItems.Remove(menuItem);
                await _context.SaveChangesAsync();

                return Ok($"Menu item with ID {id} successfully deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting menu item with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}