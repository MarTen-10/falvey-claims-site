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
    public class EmployeesController : ControllerBase
    {
        private readonly FalveyInsuranceGroupContext _context;
        public EmployeesController(FalveyInsuranceGroupContext context)
        {
            _context = context;
        }

        // For getting all employees
        [HttpGet]
        public async Task<List<EmployeeDto>> getEmployees()
        {
            var employee = await _context.Employees.Select(employee_dto => new EmployeeDto
            {
                employee_id = employee_dto.employee_id,
                name = employee_dto.name,
                title = employee_dto.title,
                email = employee_dto.email,
                phone = employee_dto.phone,
                status = employee_dto.status,
                created_at = employee_dto.created_at
            }).
            ToListAsync();

            return employee;
        }

        // For getting a specific employee
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> getEmployeeById(int id)
        {
            var employee = await _context.Employees.Where(employee_record => employee_record.employee_id == id).FirstOrDefaultAsync();
            
            if (employee == null)
                return NotFound();

            return createEmployeeDto(employee);
        }

        // For posting a new employee
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> postEmployee(Employee newEmployee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Employees.Add(newEmployee);
            await _context.SaveChangesAsync();

            var employee_dto = createEmployeeDto(newEmployee);

            return CreatedAtAction(nameof(getEmployeeById), new { id = newEmployee.employee_id }, employee_dto);
        }

        // For updating an existing employee
        [HttpPut("{id}")]
        public async Task<ActionResult> updateEmployee(int id, Employee updatedEmployee)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();
            
            employee.employee_id = updatedEmployee.employee_id;
            employee.name = updatedEmployee.name;
            employee.title = updatedEmployee.title;
            employee.email = updatedEmployee.email;
            employee.phone = updatedEmployee.phone;
            employee.status = updatedEmployee.status;
            employee.created_at = updatedEmployee.created_at;
            
            //_context.Entry(updatedEmployee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        // For patching an existing employee
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchEmployee(int id, JsonPatchDocument<Employee> patchDocument)
        {
            if (patchDocument == null)
                return BadRequest();

            // If {id} is not there, return "Not Found"
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();

            patchDocument.ApplyTo(employee, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _context.SaveChangesAsync();
            return Ok(employee);

        }
        
        // For deleting an existing employee
        [HttpDelete("{id}")]
        public async Task<ActionResult> deleteEmployee(int id)
        {
            var employee = await _context.Employees.Where(e => e.employee_id == id).FirstOrDefaultAsync();
            if (employee == null)
                return NotFound();

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private EmployeeDto createEmployeeDto(Employee employee_dto)
        {
            return new EmployeeDto
            {
                employee_id = employee_dto.employee_id,
                name = employee_dto.name,
                title = employee_dto.title,
                email = employee_dto.email,
                phone = employee_dto.phone,
                status = employee_dto.status,
                created_at = employee_dto.created_at
            };
        }

    }
}
