using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using API_service.Models;
using API_service.Services;

namespace API_service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeService _employeeService = null;

        public EmployeesController()
        {
            this._employeeService = new EmployeeService();
        }

        [HttpGet]
        [Route("/[controller]")]
        public IActionResult getAllEmployees()
        {
            // Gets the list of employees from the database
            return Ok(_employeeService.getAllEmployees());
        }

        [HttpGet]
        [Route("/[controller]/{id:int}")]
        public IActionResult getEmployee(int id)
        {
            // Gets a specific employee from the database 
            var employee = _employeeService.getEmployee(id);
            // If the employee doesn't exist in the database
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

    }
}
