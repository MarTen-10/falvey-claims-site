using FalveyInsuranceGroup.Backend.Models;
using FalveyInsuranceGroup.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;

namespace FalveyInsuranceGroup.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly FalveyInsuranceGroupContext _context;
        public UsersController(FalveyInsuranceGroupContext context)
        {
            _context = context;
        }

        // For getting all users
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            return Ok(await _context.Users
                .Include(user_index => user_index.customer) // linked customer
                .Include(user_index => user_index.employee) // linked employee
                .ToListAsync());
        }

        // For getting a specific user
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // For posting a new user
        [HttpPost]
        public async Task<ActionResult<User>> AddUser(User newUser)
        {
            if (newUser == null)
                return BadRequest();

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.user_id }, newUser);
        }

        // For updating an existing user
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.user_id = updatedUser.user_id;
            user.email = updatedUser.email;
            user.password_hash = updatedUser.password_hash;
            user.role = updatedUser.role;
            user.customer_id = updatedUser.customer_id;
            user.employee_id = updatedUser.employee_id;
            user.is_active = updatedUser.is_active;
            user.created_at = updatedUser.created_at;
            user.updated_at = updatedUser.updated_at;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // For patching an existing user
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchUser(int id, JsonPatchDocument<User> patchUser)
        {
            if (patchUser == null)
                return BadRequest();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            patchUser.ApplyTo(user, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _context.SaveChangesAsync();
            return Ok(user);
        }

        // For deleting an existing user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
