using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantZamaShared.Models;
using ZamaRestaurantWeb.Data;

namespace RestaurantZamaWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly ZamaDbContext _context;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(ZamaDbContext context, ILogger<ReservationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/reservations
        [HttpPost]
        public async Task<ActionResult<object>> CreateReservation([FromBody] Reservation reservation)
        {
            try
            {
                // Validări pentru datele de intrare
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validăm data și ora
                if (reservation.ReservationDate.Date < DateTime.Now.Date)
                {
                    return BadRequest("Nu puteți face o rezervare în trecut.");
                }

                // Validăm numărul de persoane
                if (reservation.NumberOfGuests <= 0 || reservation.NumberOfGuests > 20)
                {
                    return BadRequest("Numărul de persoane trebuie să fie între 1 și 20.");
                }

                // Validăm numărul de telefon
                if (string.IsNullOrWhiteSpace(reservation.ContactPhone))
                {
                    return BadRequest("Numărul de telefon este obligatoriu.");
                }

                // Setăm valorile implicite
                reservation.Status = "Pending";
                reservation.CreatedAt = DateTime.UtcNow;

                // Salvăm rezervarea
                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                // Returnăm detaliile rezervării
                return CreatedAtAction(
                    nameof(GetReservation),
                    new { id = reservation.Id },
                    new
                    {
                        reservation.Id,
                        reservation.ReservationDate,
                        reservation.ReservationTime,
                        reservation.NumberOfGuests,
                        reservation.ContactPhone,
                        reservation.SpecialRequests,
                        reservation.Status,
                        reservation.CreatedAt
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eroare la crearea rezervării");
                return StatusCode(500, "A apărut o eroare la salvarea rezervării. Vă rugăm încercați din nou.");
            }
        }

        // GET: api/reservations/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reservation == null)
                {
                    return NotFound($"Rezervarea cu ID-ul {id} nu a fost găsită.");
                }

                return Ok(new
                {
                    reservation.Id,
                    reservation.ReservationDate,
                    reservation.ReservationTime,
                    reservation.NumberOfGuests,
                    reservation.ContactPhone,
                    reservation.SpecialRequests,
                    reservation.Status,
                    reservation.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Eroare la obținerea rezervării {id}");
                return StatusCode(500, "A apărut o eroare la încărcarea rezervării.");
            }
        }

        // GET: api/reservations/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetUserReservations(int userId)
        {
            try
            {
                var reservations = await _context.Reservations
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.ReservationDate)
                    .ThenBy(r => r.ReservationTime)
                    .Select(r => new
                    {
                        r.Id,
                        r.ReservationDate,
                        r.ReservationTime,
                        r.NumberOfGuests,
                        r.ContactPhone,
                        r.SpecialRequests,
                        r.Status,
                        r.CreatedAt
                    })
                    .ToListAsync();

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Eroare la obținerea rezervărilor pentru utilizatorul {userId}");
                return StatusCode(500, "A apărut o eroare la încărcarea rezervărilor.");
            }
        }

        // PUT: api/reservations/{id}/cancel
        [HttpPut("{id}/cancel")]
        public async Task<ActionResult> CancelReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null)
                {
                    return NotFound($"Rezervarea cu ID-ul {id} nu a fost găsită.");
                }

                if (reservation.Status == "Cancelled")
                {
                    return BadRequest("Această rezervare este deja anulată.");
                }

                if (reservation.ReservationDate.Date < DateTime.Now.Date)
                {
                    return BadRequest("Nu puteți anula o rezervare din trecut.");
                }

                reservation.Status = "Cancelled";
                await _context.SaveChangesAsync();

                return Ok(new { message = "Rezervarea a fost anulată cu succes." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Eroare la anularea rezervării {id}");
                return StatusCode(500, "A apărut o eroare la anularea rezervării.");
            }
        }
    }
} 