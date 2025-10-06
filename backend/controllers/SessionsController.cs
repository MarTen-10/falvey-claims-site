using FalveyInsuranceGroup.Backend.Dtos;
using FalveyInsuranceGroup.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FalveyInsuranceGroup.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly FalveyInsuranceGroupContext _context;
        public SessionsController(FalveyInsuranceGroupContext context)
        {
            _context = context;
        }

        // For getting a specific session.
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SessionDto>> getSessionById(Guid id)
        {
            var session = await _context.Sessions.Where(session_record => session_record.session_id == id).FirstOrDefaultAsync();

            if (session == null)
            {
                return NotFound($"Session with ID {id} not found");
            }

            return createSessionDto(session);
        }

        // For posting a new session.
        [HttpPost]
        public async Task<ActionResult<SessionDto>> addSession(Models.Session new_session)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid Input" });
            }

            // New Guid
            new_session.session_id = Guid.NewGuid();

            // New session hash
            new_session.session_hash = newSessionHash();

            // Validate creation DateTime.
            if (new_session.created_at > DateTime.Now)
            {
                return BadRequest("Creation date cannot be future.");
            }

            if (!ipChecker(new_session.ip_address))
            {
                return BadRequest("IP address invalid.");
            }

            _context.Sessions.Add(new_session);
            await _context.SaveChangesAsync();

            var session_dto = createSessionDto(new_session);

            return CreatedAtAction(nameof(getSessionById), new { id = new_session.session_id }, session_dto);
        }

        // Session DTO.
        private SessionDto createSessionDto(Models.Session session_dto)
        {
            return new SessionDto
            {
                session_id = session_dto.session_id,
                user_id = session_dto.user_id,
                session_hash = session_dto.session_hash,
                created_at = session_dto.created_at,
                expires_at = session_dto.expires_at,
                revoked_at = session_dto.revoked_at,
                ip_address = session_dto.ip_address,
                user_agent = session_dto.user_agent,
            };
        }

        // Verify IP address.
        private Boolean ipChecker(string input)
        {
            return IPAddress.TryParse(input, out _);
        }

        // Create session hash.

        public static string newSessionHash()
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString() + DateTime.UtcNow.Ticks);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLower();
        }

    }
}
