using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantZamaShared.Models;
using ZamaRestaurantWeb.Data;

namespace RestaurantZamaWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ZamaDbContext _context;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ZamaDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var identityUser = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {
                var user = new User
                {
                    Email = model.Email,
                    Name = model.Name ?? model.Email,
                    Password = model.Password,
                    RegistrationDate = DateTime.UtcNow
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { UserId = user.Id });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var identityUser = await _userManager.FindByEmailAsync(model.Email);
            if (identityUser == null) return Unauthorized();

            var result = await _signInManager.PasswordSignInAsync(identityUser, model.Password, false, false);
            if (result.Succeeded)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                return Ok(new { UserId = user?.Id });
            }
            return Unauthorized();
        }

        [HttpGet("userid/{email}")]
        public async Task<ActionResult<int>> GetUserId(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return NotFound();
            return Ok(user.Id);
        }
    }
}