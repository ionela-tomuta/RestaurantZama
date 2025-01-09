using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantZamaShared.Models;
using ZamaRestaurantWeb.Data;
using System.Text.RegularExpressions;

namespace RestaurantZamaWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ZamaDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ZamaDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound($"Utilizatorul cu ID {id} nu a fost găsit.");
                }

                return Ok(new User
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    LoyaltyPoints = user.LoyaltyPoints,
                    RegistrationDate = user.RegistrationDate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Eroare la preluarea utilizatorului cu ID {id}");
                return StatusCode(500, "Eroare internă.");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, User updatedUser)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                {
                    return NotFound($"Utilizatorul cu ID {id} nu a fost găsit.");
                }

                // Validări specifice pentru câmpuri
                if (string.IsNullOrWhiteSpace(updatedUser.Name))
                {
                    return BadRequest("Numele este obligatoriu.");
                }

                // Validare telefon
                if (!string.IsNullOrEmpty(updatedUser.PhoneNumber) &&
                    !Regex.IsMatch(updatedUser.PhoneNumber,
                    @"^[+]?[(]?[0-9]{3}[)]?[-\s.]?[0-9]{3}[-\s.]?[0-9]{4,6}$"))
                {
                    return BadRequest("Număr de telefon invalid.");
                }

                // Actualizează doar câmpurile permise
                existingUser.Name = updatedUser.Name;
                existingUser.PhoneNumber = updatedUser.PhoneNumber;
                existingUser.Email = updatedUser.Email;

                _context.Entry(existingUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new User
                {
                    Id = existingUser.Id,
                    Name = existingUser.Name,
                    Email = existingUser.Email,
                    PhoneNumber = existingUser.PhoneNumber,
                    Role = existingUser.Role,
                    LoyaltyPoints = existingUser.LoyaltyPoints
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Eroare la actualizarea utilizatorului cu ID {id}");
                return StatusCode(500, $"Eroare internă: {ex.Message}");
            }
        }
    }
}