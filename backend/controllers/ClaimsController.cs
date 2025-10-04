using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FalveyInsuranceGroup.Db;
using FalveyInsuranceGroup.Backend.Models;
using FalveyInsuranceGroup.Backend.Dtos;
using System.Linq.Expressions;

namespace FalveyInsuranceGroup.Backend.Controllers
{
    /// <summary>
    /// Handles operations related to claim data
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        private readonly FalveyInsuranceGroupContext _context;

        public ClaimsController(FalveyInsuranceGroupContext context)
        {
            _context = context;
        }

        /// GET: api/claims

        /// <summary>
        /// Gets a list of claims
        /// </summary>
        /// <returns>A list of claims</returns>
        [HttpGet]
        public async Task<List<ClaimDto>> getClaims()
        {
            var claims = await _context.Claims
            .AsNoTracking()
            .Include(c => c.claim_policy)
            .Include(c => c.assigned_employee)
            .Include(c => c.user_uploader)
            .Select(MapToClaimDto)
            .ToListAsync();

            return claims;
        }

        /// GET: api/claims/id

        /// <summary>
        /// Gets a claim by ID
        /// </summary>
        /// <param name="id"> The id of the claim to retrieve</param>
        /// <returns>A claim dto based on provided ID</returns>
        /// <response code="200">Returns the claim</response>
        /// <response code="404">If the claim is not found</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClaimDto>> getClaim(int id)
        {
            var claim = await _context.Claims
            .AsNoTracking()
            .Include(c => c.claim_policy)
            .Include(c => c.assigned_employee)
            .Include(c => c.user_uploader)
            .FirstOrDefaultAsync(c => c.claim_id == id);

            if (claim == null) {
                return NotFound($"Claim with ID {id} not found");
            }

            return createClaimDto(claim);
        }

        /// PUT: api/claims/id

        /// <summary>
        ///  Updates an existing claim by ID
        /// </summary>
        /// <param name="id">The claim to update</param>
        /// <param name="updated_claim">Claim object that holds the new details</param>
        /// <returns>No content</returns>
        /// <response code="204">The update was a success</response>
        /// <response code="400">Validation failed</response>
        /// <response code="404">Record not found</response>
        [HttpPut("{id}")]
        public async Task<ActionResult> updateClaim(int id, [FromBody] ClaimDto dto)
        {
            if (!ModelState.IsValid) {
                return ValidationProblem(ModelState);
            }

            if (!hasValidStatus(dto.status)) {
                return BadRequest("Invalid status input");
            }

            // Ensures an existing policy is used
            if (!await hasValidPolicy(dto.policy_id)) {
                return BadRequest("The given policy ID does not exist");
            }

            if (await hasDuplicateClaimNumber(dto.claim_number)) {
                return BadRequest("The given claim number is already in use");
            }

            var claim = await _context.Claims.FindAsync(id);

            if (claim == null) {
                return NotFound($"Claim with ID {id} not found");
            }

            // Update fields
            claim.policy_id = dto.policy_id;
            claim.claim_number = dto.claim_number;
            claim.status = dto.status;
            claim.date_of_loss = dto.date_of_loss;
            claim.date_reported = dto.date_reported;
            claim.reserve_amount = dto.reserve_amount;
            claim.paid_amount = dto.paid_amount;
            claim.memo = dto.memo;
            claim.assigned_to = dto.assigned_to;
            claim.created_by = dto.created_by;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// POST: api/claims

        /// <summary>
        /// Adds a new claim
        /// </summary>
        /// <param name="dto">Claim object to add</param>
        /// <returns>The new record</returns>
        /// <response code="200">Record added successfully</response>
        /// <response code="400">Validation failed</response>
        [HttpPost]
        public async Task<ActionResult> addClaim([FromBody] ClaimDto dto)
        {
            if (!ModelState.IsValid) {
                return ValidationProblem(ModelState);
            }

            if (dto.claim_id.HasValue) {
                return BadRequest(new
                {
                    error = "Claim ID should not be provided on creation",
                    errorCode = "INVALID_CLAIM_CREATION",
                    timestamp = DateTime.UtcNow
                });
            }

            // Ensures if a given email is unique
            if (await hasDuplicateClaimNumber(dto.claim_number)) {
                return BadRequest("The given claim number is already in use");
            }
             
            // Ensures an existing policy is given
            if (!await hasValidPolicy(dto.policy_id)) {
                return BadRequest("The given policy ID does not exist");
            }

            if (!hasValidStatus(dto.status)) {
                return BadRequest("Invalid status input");
            }

            var new_claim = new Claim
            {
                policy_id = dto.policy_id,
                claim_number = dto.claim_number,
                status = dto.status ?? "Open",  // default 'Open' if null
                date_of_loss = dto.date_of_loss,
                date_reported = dto.date_reported,
                reserve_amount = dto.reserve_amount,
                paid_amount = dto.paid_amount,
                memo = dto.memo,
                assigned_to = dto.assigned_to,
                created_by = dto.created_by,
            };

            _context.Claims.Add(new_claim);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(getClaim), new { id = new_claim.claim_id }, createClaimDto(new_claim));
        }


        /// DELETE: api/claims/id

        /// <summary>
        /// Deletes an existing claim
        /// </summary>
        /// <param name="id">The claim to delete</param>
        /// <returns>No content</returns>
        /// <response code="204">Deletion was a success</response>
        /// <response code="404">Claim not found</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult> deleteClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);

            if (claim == null) {
                return NotFound($"Claim with ID {id} not found");
            }

            _context.Claims.Remove(claim);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Creates a dto using a claim entity model
        /// </summary>
        /// <param name="c">The claim entity model</param>
        /// <returns>A claim dto with the only necessary information</returns>
        private ClaimDto createClaimDto(Claim c)
        {
            return new ClaimDto
            {
                claim_id = c.claim_id,
                policy_id = c.policy_id,
                claim_number = c.claim_number,
                status = c.status,
                date_of_loss = c.date_of_loss,
                date_reported = c.date_reported,
                reserve_amount = c.reserve_amount,
                paid_amount = c.paid_amount,
                memo = c.memo,
                assigned_to = c.assigned_to,
                assigned_employee = c.assigned_employee != null ? c.assigned_employee.name : null,
                created_by = c.created_by,
                created_at = c.created_at
            };
        }

        /// <summary>
        /// Takes a Claim and maps it to a Claim dto
        /// Then it represents it as data so it can be converted to a SQL query
        /// </summary>
        private static Expression<Func<Claim, ClaimDto>> MapToClaimDto = c => new ClaimDto
        {
            claim_id = c.claim_id,
            policy_id = c.policy_id,
            claim_number = c.claim_number,
            status = c.status,
            date_of_loss = c.date_of_loss,
            date_reported = c.date_reported,
            reserve_amount = c.reserve_amount,
            paid_amount = c.paid_amount,
            memo = c.memo,
            assigned_to = c.assigned_to,
            assigned_employee = c.assigned_employee != null ? c.assigned_employee.name : null,
            created_by = c.created_by,
            created_at = c.created_at
        };

        /// <summary>
        /// Checks to see if a claim object holds a valid status
        /// </summary>
        /// <param name="status">The status of a claim</param>
        /// <returns>
        ///     True - Has a valid status
        ///     False - Has an invalid status
        /// </returns>
        private bool hasValidStatus(string status)
        {
            string[] allowed_status = { "Open", "Investigating", "Pending", "Approved", "Denied", "Closed" };

            return allowed_status.Contains(status);
        }

        /// <summary>
        /// Checks to see if a claim object holds a valid status
        /// </summary>
        /// <param name="status">The status of a claim</param>
        /// <returns>
        ///     True - Has a valid status
        ///     False - Has an invalid status
        /// </returns>

        private async Task<bool> hasValidPolicy(int policy_id)
        {
            return await _context.Policies.AnyAsync(p => p.policy_id == policy_id);
        }

        /// <summary>
        ///  Checks to see if a claim has a duplicate claim number
        /// </summary>
        /// <param name="claim_number">The claim number to check</param>
        /// <returns>
        ///     True - has a duplicate claim number
        ///     False - Has a unique claim number
        /// </returns>
        private async Task<Boolean> hasDuplicateClaimNumber(string claim_number)
        {
            return await _context.Claims.AnyAsync(c => c.claim_number == claim_number);
        }


    }

}
