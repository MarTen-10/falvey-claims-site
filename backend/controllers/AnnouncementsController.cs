using FalveyInsuranceGroup.Backend.Dtos;
using FalveyInsuranceGroup.Backend.Helpers;
using FalveyInsuranceGroup.Backend.Models;
using FalveyInsuranceGroup.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FalveyInsuranceGroup.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementsController : ControllerBase
    {
        private readonly FalveyInsuranceGroupContext _context;
        public AnnouncementsController(FalveyInsuranceGroupContext context)
        {
            _context = context;
        }

        // For getting all announcements.
        [HttpGet]
        public async Task<List<AnnouncementDto>> getAnnouncements()
        {
            var announcement = await _context.Announcements.Select(announcement_dto => new AnnouncementDto
            {
                announcement_id = announcement_dto.announcement_id,
                title = announcement_dto.title,
                body = announcement_dto.body,
                publish_at = announcement_dto.publish_at,
                expire_at = announcement_dto.expire_at,
                created_by = announcement_dto.created_by,
                created_at = announcement_dto.created_at,
            }).
            ToListAsync();
            return announcement;
        }

        // For getting a specific announcement.
        [HttpGet("{id}")]
        public async Task<ActionResult<AnnouncementDto>> getAnnouncementById(int id)
        {
            var announcement = await _context.Announcements.Where(announcement_record => announcement_record.announcement_id == id).FirstOrDefaultAsync();

            // Check if announcement exists.
            if (announcement == null)
            {
                return NotFound($"Announcement with ID {id} not found");
            }

            return createAnnouncementDto(announcement);
        }

        // For posting a new announcement.
        [HttpPost]
        public async Task<ActionResult<AnnouncementDto>> addAnnouncement(Announcement new_announcement)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // Auto-increment announcement numbers.
            if (new_announcement.announcement_id.HasValue)
            {
                return BadRequest("Announcement ID should not be provided on creation.");
            }

            // Check if only whitespace in title.
            if (Helper.checkWhitespace(new_announcement.title))
            {
                return BadRequest(new { error = "Title cannot be empty." });
            }

            if (new_announcement.created_by == 0)
            {
                return BadRequest("Creator cannot be 0 or empty.");
            }

            // Validate DateTime.
            if (new_announcement.created_at > DateTime.Now)
            {
                return BadRequest("Creation date cannot be future.");
            }

            _context.Announcements.Add(new_announcement);
            await _context.SaveChangesAsync();

            var announcement_dto = createAnnouncementDto(new_announcement);

            return CreatedAtAction(nameof(getAnnouncementById), new { id = new_announcement.announcement_id }, announcement_dto);
        }

        // For updating an existing announcement
        [HttpPut("{id}")]
        public async Task<IActionResult> updateannouncement(int id, Announcement updated_announcement)
        {
            var announcement = await _context.Announcements.FindAsync(id);

            // Check if announcement exists.
            if (announcement == null)
            {
                return NotFound($"Announcement with ID {id} not found");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid Input" });
            }

            // Check if only whitespace in title.
            if (Helper.checkWhitespace(updated_announcement.title))
            {
                return BadRequest(new { error = "Title cannot be empty." });
            }

            // Validate created at DateTime.
            if (updated_announcement.created_at > DateTime.Now)
            {
                return BadRequest("Creation date cannot be future.");
            }

            if (updated_announcement.created_by == 0)
            {
                return BadRequest("Creator cannot be 0.");
            }

            announcement.announcement_id = updated_announcement.announcement_id;
            announcement.title = updated_announcement.title;
            announcement.body = updated_announcement.body;
            announcement.publish_at = updated_announcement.publish_at;
            announcement.expire_at = updated_announcement.expire_at;
            announcement.created_by = updated_announcement.created_by;
            announcement.created_at = updated_announcement.created_at;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // For deleting an existing announcement.
        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteAnnouncement(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);

            // Check if announcement exists.
            if (announcement == null)
            {
                return NotFound($"Announcement with ID {id} not found");
            }

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Announcement DTO.
        private AnnouncementDto createAnnouncementDto(Announcement announcement_dto)
        {
            return new AnnouncementDto
            {
                announcement_id = announcement_dto.announcement_id,
                title = announcement_dto.title,
                body = announcement_dto.body,
                publish_at = announcement_dto.publish_at,
                expire_at = announcement_dto.expire_at,
                created_by = announcement_dto.created_by,
                created_at = announcement_dto.created_at,
            };
        }

    }
}
