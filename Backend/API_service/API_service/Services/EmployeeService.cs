using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_service.Models;

namespace API_service.Services
{
    public class EmployeeService
    {
        private CoreDbContext _context = new CoreDbContext();

        public List<Employee> getAllEmployees()
        {
            return _context.Employees.ToList();
        }

        public Employee getEmployee(int id)
        {
            return _context.Employees.Find(id);
        }

    }
}
