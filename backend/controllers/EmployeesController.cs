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

        /// GET: api/employees

        /// <summary>
        /// Gets a list of employees
        /// </summary>
        /// <returns>A list of employees</returns>
        [HttpGet]
        public async Task<List<EmployeeDto>> getEmployees()
        {
            var employees = await _context.Employees
            .AsNoTracking()
            .Select(e => new EmployeeDto
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

        /// GET: api/employees/id

        /// <summary>
        /// Gets an employee by ID
        /// </summary>
        /// <param name="id"> The id of the employee to retrieve</param>
        /// <returns>An employee dto based on provided ID</returns>
        /// <response code="200">Returns the employee</response>
        /// <response code="404">If the employee is not found</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> getEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound(($"Employee with ID {id} not found"));
            }

            return Ok(createEmployeeDto(employee));
        }

        /// POST: api/employees

        /// <summary>
        /// Adds a new employee
        /// </summary>
        /// <param name="employee">Employee object to add</param>
        /// <returns>The new employee</returns>
        /// <response code="200">Employee added successfully</response>
        /// <response code="400">Validation failed</response>
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> addEmployee(Employee employee)
        {
            // Checks to see if all required inputs are provided
            if (!ModelState.IsValid || !hasValidStatus(employee.status))
            {
                return BadRequest("Invalid inputs");
            }

            // Ensures that ID is not provided
            if (employee.employee_id.HasValue)
            {
                return BadRequest(new
                {
                    error = "Employee ID should not be provided on creation",
                    errorCode = "INVALID_EMPLOYEE_CREATION",
                    timestamp = DateTime.UtcNow
                });
            }

            // Ensures if a given email is unique
            if (employee.email != null)
            {
                if (hasDuplicateEmail(employee.email))
                {
                    return BadRequest("The given email is already in use");
                }
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();


            return CreatedAtAction(nameof(getEmployee), new { id = employee.employee_id }, createEmployeeDto(employee));
        }


        /// PUT: api/employees/id

        /// <summary>
        ///  Updates an existing employee by ID
        /// </summary>
        /// <param name="id">The employee to update</param>
        /// <param name="updated_employee">Employee object that holds the new details</param>
        /// <returns>No content</returns>
        /// <response code="204">The update was a success</response>
        /// <response code="400">Validation failed</response>
        /// <response code="404">Employee not found</response>
        [HttpPut("{id}")]
        public async Task<ActionResult> updateEmployee(int id, Employee updated_employee)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (!hasValidStatus(updated_employee.status))
            {
                return BadRequest("Invalid status input");
            }

            // Ensures if a given email is unique
            if (updated_employee.email != null)
            {
                if (hasDuplicateEmail(updated_employee.email))
                {
                    return BadRequest("The given email is already in use");
                }
            }

            var existing_employee = await _context.Employees.FindAsync(id);

            if (existing_employee == null)
            {
                return NotFound($"Employee with {id} not found");
            }

            // Update fields
            existing_employee.name = updated_employee.name;
            existing_employee.title = updated_employee.title;
            existing_employee.email = updated_employee.email;
            existing_employee.phone = updated_employee.phone;
            existing_employee.status = updated_employee.status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// DELETE: api/employees/id

        /// <summary>
        /// Deletes an existing employee
        /// </summary>
        /// <param name="id">The employee to delete</param>
        /// <returns>No content</returns>
        /// <response code="204">Deletion was a success</response>
        /// <response code="404">Employee not found</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult> deleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound($"Employee with {id} not found");
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Creates a dto using an Employee entity model
        /// </summary>
        /// <param name="e">The employee entity model</param>
        /// <returns>An employee dto with only necessary information</returns>
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

        /// <summary>
        /// Checks to see if an employee object holds a valid status
        /// </summary>
        /// <param name="status">The status of the employee</param>
        /// <returns>
        ///     True - Has a valid status
        ///     False - Has an invalid status
        /// </returns>
        private bool hasValidStatus(string status)
        {
            string[] allowed_status = { "Active", "Inactive", "Leave", "Terminated" };

            return allowed_status.Contains(status);
        }

        /// <summary>
        ///  Checks to see if an employee has a duplicate email
        /// </summary>
        /// <param name="new_email">The email to check</param>
        /// <returns>
        ///     True - has a duplicate email
        ///     False - Has a unique email
        /// </returns>
        private Boolean hasDuplicateEmail(string new_email)
        {
            // Assigns a IQueryable that contains employees with the same email
            // Assigns an empty IQueryable when there are no same emails found
            var entity_with_email = _context.Employees.Where(e => e.email == new_email);

            // Checks to see if IQueryable is empty
            return entity_with_email.Any();
        }

    }
}

