using FalveyInsuranceGroup.Backend.Models;
using FalveyInsuranceGroup.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;

namespace FalveyInsuranceGroup.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PoliciesController : ControllerBase
    {
        private readonly FalveyInsuranceGroupContext _context;
        public PoliciesController(FalveyInsuranceGroupContext context)
        {
            _context = context;
        }

        // For getting all policies
        [HttpGet]
        public async Task<ActionResult<List<Policy>>> GetPolicies()
        {
            return Ok(await _context.Policies
                .Include(p => p.customer) // linked customer
                .Include(p => p.manager)  // linked manager
                .ToListAsync());
        }

        // For getting a specific policy
        [HttpGet("{id}")]
        public async Task<ActionResult<Policy>> GetPolicyById(int id)
        {
            var policy = await _context.Policies.FindAsync(id);
            if (policy == null)
                return NotFound();

            return Ok(policy);
        }
        
        // For posting a new policy
        [HttpPost]
        public async Task<ActionResult<Policy>> AddPolicy(Policy newPolicy)
        {
            if (newPolicy == null)
                return BadRequest();

            _context.Policies.Add(newPolicy);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPolicyById), new { id = newPolicy.policy_id }, newPolicy);
        }
        
        // For updating an existing policy
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePolicy(int id, Policy updatedPolicy)
        {
            var policy = await _context.Policies.FindAsync(id);
            if (policy == null)
                return NotFound();

            policy.policy_id = updatedPolicy.policy_id;
            policy.account_number = updatedPolicy.account_number;
            policy.customer_id = updatedPolicy.customer_id;
            policy.manager_id = updatedPolicy.manager_id;
            policy.policy_type = updatedPolicy.policy_type;
            policy.status = updatedPolicy.status;
            policy.start_date = updatedPolicy.start_date;
            policy.end_date = updatedPolicy.end_date;
            policy.exposure_amount = updatedPolicy.exposure_amount;
            policy.loc_addr1 = updatedPolicy.loc_addr1;
            policy.loc_addr2 = updatedPolicy.loc_addr2;
            policy.loc_city = updatedPolicy.loc_city;
            policy.loc_state = updatedPolicy.loc_state;
            policy.loc_zip = updatedPolicy.loc_zip;
            policy.created_at = updatedPolicy.created_at;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        // For patching an existing policy
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchPolicy(int id, JsonPatchDocument<Policy> patchPolicy)
        {
            if (patchPolicy == null)
                return BadRequest();

            var policy = await _context.Policies.FindAsync(id);
            if (policy == null)
                return NotFound();

            patchPolicy.ApplyTo(policy, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _context.SaveChangesAsync();
            return Ok(policy);
        }

        // For deleting an existing policy
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var policy = await _context.Policies.FindAsync(id);
            if (policy == null)
                return NotFound();

            _context.Policies.Remove(policy);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
