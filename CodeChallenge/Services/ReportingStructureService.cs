using CodeChallenge.Models;
using CodeChallenge.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CodeChallenge.Services
{
    public class ReportingStructureService : IReportingStructureService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<ReportingStructureService> _logger;

        public ReportingStructureService(ILogger<ReportingStructureService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        /// <summary>
        /// Returns a Reporting Structure with the Employee object for the provided id
        /// and a count of their total Reports
        /// </summary>
        /// <param name="id">An string representing the id of an employee</param>
        /// <returns>
        /// ReportingStructure object with Employee and count of Reports
        /// </returns>
        public ReportingStructure GetById(string id)
        {
            Employee firstEmp = _employeeRepository.GetById(id);
            if (firstEmp == null) return null;
            ReportingStructure rs = new ReportingStructure();
            rs.Employee = firstEmp;
            //if the target has no reports, set to 0 and return
            if (firstEmp.DirectReports == null) 
            {
                rs.NumberOfReports = 0;
                return rs;
            }
            //If the target has at least one report, iterate down
            //the tree to total up the structure
            else
            {
                int reportCount = SumReports(firstEmp);
                reportCount -= 1; 
                //need to subtract one since the function counts the orignal employee
                //TODO: would like to revisit this logic
                rs.NumberOfReports = reportCount;
                return rs;
            }
        }

        /// <summary>
        /// Sums up the number of reports underneath a given employee, called recursively
        /// to traverse the entire Reporting Structure
        /// </summary>
        /// <param name="emp">The employee whose reports are to be summed up</param>
        /// <returns>
        /// Integer representing the number of total reports under the provided employee
        /// </returns>
        private int SumReports(Employee emp)
        {
            //If the employee is at the base of a tree, they still count for one
            if (!emp.DirectReports.Any()) return 1;
            //if a report has reports of their own, recursively call this function to sum up the total
            else
            {
                int totalReports = 1;
                foreach (Employee report in emp.DirectReports)
                {
                    //Do a GetById here to ensure reports of reports are loaded
                    Employee reportOfReport = _employeeRepository.GetById(report.EmployeeId);
                    totalReports += SumReports(reportOfReport);
                }
                return totalReports;
            }

        }
    }
}
