using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DemoEmployeeDb.Models;
using DemoEmployeeDb.Services;

namespace DemoEmployeeDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            return await _employeeService.GetEmployeesAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(string id, [FromQuery] string dType)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id, dType);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
        {
            if (employee == null || string.IsNullOrWhiteSpace(employee.Id) || string.IsNullOrWhiteSpace(employee.DType))
            {
                return BadRequest("Employee ID and dType are required.");
            }

            await _employeeService.AddEmployeeAsync(employee);
            return Ok("Employee created successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(string id, [FromBody] Employee employee)
        {
            if (employee == null || id != employee.Id)
            {
                return BadRequest("Employee ID mismatch.");
            }

            var updated = await _employeeService.UpdateEmployeeAsync(id, employee);
            if (!updated)
            {
                return NotFound();
            }

            return Ok("Employee updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(string id, [FromQuery] string dType)
        {
            await _employeeService.DeleteEmployeeAsync(id, dType);
            return Ok("Employee deleted successfully.");
        }

    }
}
