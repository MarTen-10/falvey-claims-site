using FalveyInsuranceGroup.Backend.Models;
using FalveyInsuranceGroup.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using FalveyInsuranceGroup.Backend.Dtos;


namespace FalveyInsuranceGroup.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerRecordsController : ControllerBase
    {
        private readonly FalveyInsuranceGroupContext _context;

        public CustomerRecordsController(FalveyInsuranceGroupContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<CustomerRecordDto>>> getCustomerRecords()
        {
            var customer_record = await _context.CustomerRecords
            .Include(r => r.uploaded_by_employee)
            .Select(r => new CustomerRecordDto
            {
                record_id = r.record_id,
                record_name = r.record_name,
                url = r.url,
                uploaded_by = r.uploaded_by,
                uploaded_by_name = r.uploaded_by_employee != null ? r.uploaded_by_employee.name : null,
                uploaded_at = r.uploaded_at,
                attached_to_type = r.attached_to_type,
                attached_to_id = r.attached_to_id,
                description = r.description
            }).ToListAsync();


            return Ok(customer_record);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerRecordDto>> getCustomerRecord(int id)
        {
            var customer_record = await getCustomerRecordById(id);

            if (customer_record == null)
            {
                return NotFound();
            }

            return Ok(createCustomerRecordDto(customer_record));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerRecordDto>> postCustomerRecord(CustomerRecord customer_record)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _context.CustomerRecords.Add(customer_record);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(getCustomerRecord), new { id = customer_record.record_id }, createCustomerRecordDto(customer_record));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> updateCustomerRecord(int id, CustomerRecord customer_record)
        {
            if (id != customer_record.record_id)
            {
                return BadRequest();
            }

            _context.Entry(customer_record).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> deleteCustomerRecord(int id)
        {
            var customer_record = await getCustomerRecordById(id);

            if (customer_record == null)
            {
                return NotFound();
            }

            _context.CustomerRecords.Remove(customer_record);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        private async Task<CustomerRecord?> getCustomerRecordById(int id)
        {
            var customer_record = await _context.CustomerRecords
            .Include(r => r.uploaded_by_employee)
            .FirstOrDefaultAsync(r => r.record_id == id);

            return customer_record != null ? customer_record : null;
        }
    

        private CustomerRecordDto createCustomerRecordDto(CustomerRecord r)
        {
            return new CustomerRecordDto
            {
                record_id = r.record_id,
                record_name = r.record_name,
                url = r.url,
                uploaded_by = r.uploaded_by,
                uploaded_by_name = r.uploaded_by_employee != null ? r.uploaded_by_employee.name : null,
                uploaded_at = r.uploaded_at,
                attached_to_type = r.attached_to_type,
                attached_to_id = r.attached_to_id,
                description = r.description
            };
        }

    }
}