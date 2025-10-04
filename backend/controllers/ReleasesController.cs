using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FalveyProject.database;
using FalveyProject.backend.models;
using System.Text.RegularExpressions;


namespace FalveyProject.backend.controllers
{
    /// <summary>
    /// Handles operations related to application releases
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReleasesController : ControllerBase
    {
        private readonly BaseContext _context;
        public ReleasesController(BaseContext context)
        {
            _context = context;
        }

        /// GET: api/releases
    
        /// <summary>
        /// Gets a list of releases
        /// </summary>
        /// <returns>A list of releases</returns>
        [HttpGet]
        public async Task<ActionResult<Release>> getReleases()
        {
            return Ok(await _context.Releases.AsNoTracking().ToListAsync());
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

            if (release == null)
            {
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
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
            
            // Format is v[MAJOR].[MINOR].[PATCH]
            var version_regex = "^v\\d+\\.\\d+\\.\\d+$";

            // Checks to see if release version follows Semantic Versioning 
            if (!Regex.IsMatch(version_regex, release.version))
            {
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
        /// <param name="updated_elease">Release object that holds the new details</param>
        /// <returns>No content</returns>
        /// <response code="204">The update was a success</response>
        /// <response code="400">Validation failed</response>
        /// <response code="404">Release not found</response>
        [HttpPut("{id}")]
        public async Task<ActionResult> updateRelease(string id, Release updated_release)
        {

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

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

            if (release == null)
            {
                return NotFound($"Release with ID {id} not found");
            }

            _context.Releases.Remove(release);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}