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
    public class CustomersController : ControllerBase 
    {
        private readonly FalveyInsuranceGroupContext _context;
        public CustomersController(FalveyInsuranceGroupContext context)
        {
            _context = context;
        }

        // For getting all customers.
        [HttpGet]
        public async Task<List<CustomerDto>> getCustomers()
        {
            var customer = await _context.Customers.Select(customer_dto => new CustomerDto
            {
                customer_id = customer_dto.customer_id,
                name = customer_dto.name,
                email = customer_dto.email,
                phone = customer_dto.phone,
                addr_line1 = customer_dto.addr_line1,
                addr_line2 = customer_dto.addr_line2,
                city = customer_dto.city,
                state_code = customer_dto.state_code,
                zip_code = customer_dto.zip_code,
                created_at = customer_dto.created_at
            }).
            ToListAsync();
            return customer;
        }

        // For getting a specific customer.
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> getCustomerById(int id)
        {
            var customer = await _context.Customers.Where(customer_record => customer_record.customer_id == id).FirstOrDefaultAsync();

            // Check if custoemr exists.
            if (customer == null)
            {
                return NotFound($"Customer with ID {id} not found");
            }

            return createCustomerDto(customer);
        }

        // For posting a new customer.
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> addCustomer(Customer new_customer)
        {   
            // Validate input. 
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid Input" });
            }

            // Auto-increment customer numbers.
            if (new_customer.customer_id.HasValue)
            {
                return BadRequest("Employee ID should not be provided on creation.");
            }

            // Validate no whitespace in email.
            if (new_customer.email.Contains(" "))
            {
                return BadRequest(new { error = "Remove whitespace from email." });
            }

            // Validate state code.
            if (!Helper.checkStateCode(new_customer.state_code))
            {
                return BadRequest(new { error = 
                    "Please enter a two-letter code for a state, the District of Columbia, or the five US territories." });
            }

            // Validate created time.
            if (new_customer.created_at > DateTime.Now)
            {
                return BadRequest("Creation date cannot be future.");
            }

            _context.Customers.Add(new_customer);
            await _context.SaveChangesAsync();

            var customer_dto = createCustomerDto(new_customer);

            return CreatedAtAction(nameof(getCustomerById), new { id = new_customer.customer_id }, customer_dto);
        }

        // For updating an existing customer
        [HttpPut("{id}")]
        public async Task<IActionResult> updateCustomer(int id, Customer updated_customer)
        {
            var customer = await _context.Customers.FindAsync(id);

            // Check if custoemr exists.
            if (customer == null)
            {
                return NotFound($"Customer with ID {id} not found");
            }

            // Validate input.
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid Input" });
            }


            // Validate no whitespace in email.
            if (updated_customer.email.Contains(" "))
            {
                return BadRequest(new { error = "Remove whitespace from email." });
            }


            // Validate state code.
            if (!Helper.checkStateCode(updated_customer.state_code))
            {
                return BadRequest(new { error = "Please enter a two-letter code for a state, the District of Columbia, or the five US territories." });
            }


            // Validate created time.
            if (updated_customer.created_at > DateTime.Now)
            {
                return BadRequest("Creation date cannot be future.");
            }

            customer.customer_id = updated_customer.customer_id;
            customer.name = updated_customer.name;
            customer.email = updated_customer.email;
            customer.phone = updated_customer.phone;
            customer.addr_line1 = updated_customer.addr_line1;
            customer.addr_line2 = updated_customer.addr_line2;
            customer.city = updated_customer.city;
            customer.state_code = updated_customer.state_code;
            customer.zip_code = updated_customer.zip_code;
            customer.created_at = updated_customer.created_at;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // For deleting an existing customer.
        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            // Check if custoemr exists.
            if (customer == null)
            {
                return NotFound($"Customer with ID {id} not found");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Customer DTO.
        private CustomerDto createCustomerDto(Customer customer_dto)
        {
            return new CustomerDto
            {
                customer_id = customer_dto.customer_id,
                name = customer_dto.name,
                email = customer_dto.email,
                phone = customer_dto.phone,
                addr_line1 = customer_dto.addr_line1,
                addr_line2 = customer_dto.addr_line2,
                city = customer_dto.city,
                state_code = customer_dto.state_code,
                zip_code = customer_dto.zip_code,
                created_at = customer_dto.created_at
            };
        }

    }
}
