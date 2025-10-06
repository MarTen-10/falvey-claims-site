using FalveyInsuranceGroup.Backend.Dtos;
using FalveyInsuranceGroup.Backend.Helpers;
using FalveyInsuranceGroup.Backend.Models;
using FalveyInsuranceGroup.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;

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
        public async Task<List<PolicyDto>> getPolicies()
        {
            var policy = await _context.Policies
                .Include(policy_dto => policy_dto.customer) // linked customer
                .Include(policy_dto => policy_dto.manager)  // linked manager
                .Select(policy_dto => new PolicyDto
            {
                policy_id = policy_dto.policy_id,
                account_number = policy_dto.account_number,
                customer_id = policy_dto.customer_id,
                customer_name = policy_dto.customer != null ? policy_dto.customer.name : null,
                manager_id = policy_dto.manager_id,
                manager_name = policy_dto.manager != null ? policy_dto.manager.name : null,
                policy_type = policy_dto.policy_type,
                status = policy_dto.status,
                start_date = policy_dto.start_date,
                end_date = policy_dto.end_date,
                exposure_amount = policy_dto.exposure_amount,
                loc_addr1 = policy_dto.loc_addr1,
                loc_addr2 = policy_dto.loc_addr2,
                loc_city = policy_dto.loc_city,
                loc_state = policy_dto.loc_state,
                loc_zip = policy_dto.loc_zip,
                created_at = policy_dto.created_at
            }).
            ToListAsync();
            return policy;
        }

        // For getting a specific policy
        [HttpGet("{id}")]
        public async Task<ActionResult<PolicyDto>> getPolicyById(int id)
        {
            var policy = await _context.Policies.Where(policy_record => policy_record.policy_id == id).FirstOrDefaultAsync();

            if (policy == null)
            {
                return NotFound($"Policy with ID {id} not found");
            }

            return createPolicyDto(policy);
        }
        
        // For posting a new policy
        [HttpPost]
        public async Task<ActionResult<PolicyDto>> addPolicy(Policy new_policy)
        {
            // Check for valid input.
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid Input" });
            }

            // Validate policy type.
            if (!checkType(new_policy.policy_type))
            {
                return BadRequest(new { error = "Please enter a valid policy type." });
            }

            // Validate policy status.
            if (!checkStatus(new_policy.status))
            {
                return BadRequest(new { error = "Please enter a valid status." });
            }

            // Validate state code.
            if (!Helper.checkStateCode(new_policy.loc_state))
            {
                return BadRequest(new { error = "Please enter a two-letter code for a state, the District of Columbia, or the five US territories." });
            }

            _context.Policies.Add(new_policy);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(getPolicyById), new { id = new_policy.policy_id }, new_policy);
        }
        
        // For updating an existing policy
        [HttpPut("{id}")]
        public async Task<IActionResult> updatePolicy(int id, Policy updated_policy)
        {
            var policy = await _context.Policies.FindAsync(id);

            // Check if policy exists.
            if (policy == null)
            {
                return NotFound($"Policy with ID {id} not found");
            }

            // Check for invalid input.
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid Input" });
            }

            // Validate policy type.
            if (!checkType(updated_policy.policy_type))
            {
                return BadRequest(new { error = "Please enter a valid policy type." });
            }

            // Validate policy status.
            if (!checkStatus(updated_policy.status))
            {
                return BadRequest(new { error = "Please enter a valid status." });
            }


            // Validate state code.
            if (!Helper.checkStateCode(updated_policy.loc_state))
            {
                return BadRequest(new { error = "Please enter a two-letter code for a state, the District of Columbia, or the five US territories." });
            }


            // Validate created at DateTime.
            if (updated_policy.created_at > DateTime.Now)
            {
                return BadRequest("Creation date cannot be future.");
            }

            policy.policy_id = updated_policy.policy_id;
            policy.account_number = updated_policy.account_number;
            policy.customer_id = updated_policy.customer_id;
            policy.customer = updated_policy.customer;
            policy.manager_id = updated_policy.manager_id;
            policy.manager = updated_policy.manager;
            policy.policy_type = updated_policy.policy_type;
            policy.status = updated_policy.status;
            policy.start_date = updated_policy.start_date;
            policy.end_date = updated_policy.end_date;
            policy.exposure_amount = updated_policy.exposure_amount;
            policy.loc_addr1 = updated_policy.loc_addr1;
            policy.loc_addr2 = updated_policy.loc_addr2;
            policy.loc_city = updated_policy.loc_city;
            policy.loc_state = updated_policy.loc_state;
            policy.loc_zip = updated_policy.loc_zip;
            policy.created_at = updated_policy.created_at;

            await _context.SaveChangesAsync();

            return NoContent();
        }
     
        // For deleting an existing policy
        [HttpDelete("{id}")]
        public async Task<IActionResult> deletePolicy(int id)
        {
            var policy = await _context.Policies.FindAsync(id);

            // Check if policy exists.
            if (policy == null)
            {
                return NotFound($"Policy with ID {id} not found");
            }

            _context.Policies.Remove(policy);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private PolicyDto createPolicyDto(Policy policy_dto)
        {
            return new PolicyDto
            {
                policy_id = policy_dto.policy_id,
                account_number = policy_dto.account_number,
                customer_id = policy_dto.customer_id,
                customer_name = policy_dto.customer != null ? policy_dto.customer.name : null,
                manager_id = policy_dto.manager_id,
                manager_name = policy_dto.manager != null ? policy_dto.manager.name : null,
                policy_type = policy_dto.policy_type,
                status = policy_dto.status,
                start_date = policy_dto.start_date,
                end_date = policy_dto.end_date,
                exposure_amount = policy_dto.exposure_amount,
                loc_addr1 = policy_dto.loc_addr1,
                loc_addr2 = policy_dto.loc_addr2,
                loc_city = policy_dto.loc_city,
                loc_state = policy_dto.loc_state,
                loc_zip = policy_dto.loc_zip,
                created_at = policy_dto.created_at
            };
        }

        // Check policy type.
        private Boolean checkType(string type)
        {
            Boolean stringMatches = false;
            string[] types = { "Auto", "Property", "Liability", "Commercial", "Marine", "Other" };

            for (int i = 0; i < types.Length; ++i)
            {
                if (type == types[i])
                {
                    stringMatches = true;
                }
            }
            return stringMatches;
        }

        // Check policy status.
        private Boolean checkStatus(string status)
        {
            Boolean stringMatches = false;
            string[] statuses = { "Active", "Pending", "Cancelled", "Expired" };

            for (int i = 0; i < statuses.Length; ++i)
            {
                if (status == statuses[i])
                {
                    stringMatches = true;
                }
            }
            return stringMatches;
        }
    }
}
