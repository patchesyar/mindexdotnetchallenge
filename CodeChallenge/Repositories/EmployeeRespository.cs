using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            if(employee.DirectReports != null)
            {
                employee.DirectReports = ListReports(employee);
            }
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            return _employeeContext.Employees.Where(e => e.EmployeeId == id)
                .Include(e => e.DirectReports) 
                .ToList() 
                .FirstOrDefault();
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }

        /// <summary>
        /// Converts a list of employeeIDs to complete Employee objects
        /// used when adding new employees via API
        /// </summary>
        /// <param name="employee">The Employee being added with invalid reports list</param>
        /// <returns>list of employee objects with info filled out</returns>
        private List<Employee> ListReports(Employee employee)
        {
            List<Employee> result = new List<Employee>();
            foreach(Employee e in  employee.DirectReports)
            {
                result.Add(GetById(e.EmployeeId));
            }
            return result;
        }
    }
}
