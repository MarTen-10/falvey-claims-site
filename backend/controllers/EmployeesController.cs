using Microsoft.AspNetCore.Mvc;
using FalveyProject.backend.dtos;
using FalveyProject.database;
using Microsoft.EntityFrameworkCore;
using FalveyProject.backend.models;

namespace FalveyProject.backend.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly BaseContext _context;

        public EmployeesController(BaseContext context)
        {
            _context = context;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<List<EmployeeDto>> getEmployees()
        {
            var employees = await _context.Employees.Select(e => new EmployeeDto
            {
                employee_id = e.employee_id,
                name = e.name,
                title = e.title,
                email = e.email,
                phone = e.phone
            }).
            ToListAsync();

            return employees;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> getEmployee(int id)
        {
            var employee = await _context.Employees.Where(e => e.employee_id == id).FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound();
            }

            return createEmployeeDto(employee);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> postEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var employee_dto = createEmployeeDto(employee);

            return CreatedAtAction(nameof(getEmployee), new { employee.employee_id }, employee_dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> updateEmployee(int id, Employee employee)
        {
            if (id != employee.employee_id)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> deleteEmployee(int id)
        {
            var employee = await _context.Employees.Where(e => e.employee_id == id).FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private EmployeeDto createEmployeeDto(Employee e)
        {
            return new EmployeeDto
            {
                employee_id = e.employee_id,
                name = e.name,
                title = e.title,
                email = e.email,
                phone = e.phone
            };
        }
    }
}