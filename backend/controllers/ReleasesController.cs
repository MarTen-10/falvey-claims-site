using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FalveyInsuranceGroup.Db;
using FalveyInsuranceGroup.Backend.Models;
using FalveyInsuranceGroup.Backend.Helpers;

namespace FalveyInsuranceGroup.Backend.Controllers
{
    /// <summary>
    /// Handles operations related to application releases
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReleasesController : ControllerBase
    {
        private readonly FalveyInsuranceGroupContext _context;
        private readonly InputService _service;
        private readonly ILogger<ReleasesController> _logger;
        
        public ReleasesController(
            FalveyInsuranceGroupContext context, 
            InputService service,
            ILogger<ReleasesController> logger)
        {
            _context = context;
            _service = service;
            _logger = logger;
        }

        /// GET: api/releases
    
        /// <summary>
        /// Gets a list of releases
        /// </summary>
        /// <returns>A list of releases</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Release>>> getReleases()
        {
            try
            {
                _logger.LogInformation("Fetching releases from database...");
                
                var releases = await _context.Releases
                    .OrderByDescending(r => r.start_date)
                    .ToListAsync();
                
                _logger.LogInformation("Found {Count} releases", releases.Count);
                
                return Ok(new
                {
                    success = true,
                    count = releases.Count,
                    data = releases
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching releases");
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        /// GET: api/releases/id

        /// <summary>
        /// Gets a release by ID
        /// </summary>
        /// <param name="id"> The id of the release to retrieve</param>
        /// <returns>A release based on provided ID</returns>
        /// <response code="200">Returns the release</response>
        /// <response code="404">If the release is not found</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Release>> getRelease(string id)
        {
            var release = await _context.Releases.FindAsync(id);

            if (release == null) {
                return NotFound($"Release with ID {id} not found");
            }

            return Ok(release); // 200
        }

        /// POST: api/releases

        /// <summary>
        /// Adds a new release
        /// </summary>
        /// <param name="release"> Release object to add</param>
        /// <returns>The new release</returns>
        /// <response code="200">Release added successfully</response>
        /// <response code="400">Validation failed</response>
        [HttpPost]
        public async Task<ActionResult<Release>> addRelease(Release release)
        {
            
            // Checks to see if release version follows Semantic Versioning 
            if (!_service.hasValidVersion(release.version)) {
                return BadRequest(new
                {
                    error = "Version number is formatted incorrectly",
                    errorCode = "INVALID_VERSION_CREATION",
                    timestamp = DateTime.UtcNow
                });
            }

            _context.Releases.Add(release);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(getRelease), new { id = release.version }, release); // 200
        }


        /// PUT: api/releases
    
        /// <summary>
        ///  Updates an existing release by version
        /// </summary>
        /// <param name="id">The version to update</param>
        /// <param name="updated_release">Release object that holds the new details</param>
        /// <returns>No content</returns>
        /// <response code="204">The update was a success</response>
        /// <response code="400">Validation failed</response>
        /// <response code="404">Release not found</response>
        [HttpPut("{id}")]
        public async Task<ActionResult> updateRelease(string id, Release updated_release)
        {
            var existing_release = await _context.Releases.FindAsync(id);
            
            // Checks to see if release exists
            if (existing_release == null) {
                return NotFound($"Release with ID {id} not found");
            }

            // Update fields
            existing_release.rollout_date = updated_release.rollout_date;
            existing_release.start_date = updated_release.start_date;
            existing_release.complete_date = updated_release.complete_date;
            existing_release.notes = updated_release.notes;
            existing_release.hotfix_notes = updated_release.hotfix_notes;

            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }

        /// DELETE: api/releases

        /// <summary>
        /// Deletes an existing release
        /// </summary>
        /// <param name="id">The version to delete</param>
        /// <returns>No content</returns>
        /// <response code="204">Deletion was a success</response>
        /// <response code="404">Release not found</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult> deleteRelease(string id)
        {
            var release = await _context.Releases.FindAsync(id);

            if (release == null) {
                return NotFound($"Release with ID {id} not found");
            }

            _context.Releases.Remove(release);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}