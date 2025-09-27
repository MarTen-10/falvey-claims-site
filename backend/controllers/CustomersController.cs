using FalveyInsuranceGroup.Backend.Models;
using FalveyInsuranceGroup.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;

namespace FalveyInsuranceGroup.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase 
    {
        private readonly FalveyInsuranceGroupContext _context;
        public CustomersController(FalveyInsuranceGroupContext context)
        {
            _context = context;
        }

        // For getting all customers
        [HttpGet]
        public async Task<ActionResult<List<Customer>>> GetCustomers()
        {
            return Ok(await _context.Customers.ToListAsync());
        }

        // For getting a specific customer
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        // For posting a new customer
        [HttpPost]
        public async Task<ActionResult<Customer>> AddCustomer(Customer newCustomer)
        {
            if (newCustomer == null)
                return BadRequest();

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomer.customer_id }, newCustomer);
        }

        // For updating an existing customer
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, Customer updatedCustomer)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            customer.customer_id = updatedCustomer.customer_id;
            customer.name = updatedCustomer.name;
            customer.email = updatedCustomer.email;
            customer.phone = updatedCustomer.phone;
            customer.addr_line1 = updatedCustomer.addr_line1;
            customer.addr_line2 = updatedCustomer.addr_line2;
            customer.city = updatedCustomer.city;
            customer.state_code = updatedCustomer.state_code;
            customer.zip_code = updatedCustomer.zip_code;
            customer.created_at = updatedCustomer.created_at;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // For patching an existing customer
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchCustomer(int id, JsonPatchDocument<Customer> patchDocument)
        {
            if (patchDocument == null)
                return BadRequest();

            // If {id} is not there, return "Not Found"
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            patchDocument.ApplyTo(customer, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _context.SaveChangesAsync();
            return Ok(customer);

        }

        // For deleting an existing customer
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
