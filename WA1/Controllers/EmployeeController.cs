using Microsoft.AspNetCore.Mvc;
using WA1.Entities;
using WA1.UserInterface;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WA1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployee _employeeRepo;

        public EmployeeController(IEmployee employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        // POST: api/Employee
        [HttpPost]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            if (!ModelState.IsValid) // Validate the model
            {
                return BadRequest(ModelState);
            }

            await _employeeRepo.AddEmployeeAsync(employee);
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        // GET: api/Employee/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _employeeRepo.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        // GET: api/Employee
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeRepo.GetAllEmployeesAsync();
            return Ok(employees);
        }

        // PUT: api/Employee/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            
            var existingEmployee = await _employeeRepo.GetEmployeeByIdAsync(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            existingEmployee.Name = employee.Name;
            existingEmployee.DeptId = employee.DeptId;

           
            // Save changes
            await _employeeRepo.UpdateEmployeeAsync(existingEmployee);

            return NoContent(); 
        }




        // DELETE: api/Employee/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _employeeRepo.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            await _employeeRepo.DeleteEmployeeAsync(id);
            return NoContent(); 
        }
    }
}
