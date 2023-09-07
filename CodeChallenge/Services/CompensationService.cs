using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICompensationRepository _compensationRepository;
        private readonly ILogger<CompensationService> _logger;

        public CompensationService(ILogger<CompensationService> logger, IEmployeeRepository employeeRepository, ICompensationRepository compensationRepository)
        {
            _employeeRepository = employeeRepository;
            _compensationRepository = compensationRepository;
            _logger = logger;
        }

        /// <summary>
        /// Gets a Compensation for the Employee with the provided id
        /// </summary>
        /// <param name="id">The id of the employee</param>
        /// <returns>
        /// Compensation object with information about the queried employee
        /// </returns>
        public Compensation GetById(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                return _compensationRepository.GetById(id);
            }

            return null;
        }

        // TODO: Remember I brought in employeeRepository so I can make sure that you don't post a comp for a nonexistent employee
        public Compensation Upsert(Compensation compensation)
        {
            Compensation compensationToUpdate = GetById(compensation.EmployeeId);
            //If the Compensation does not exist for the employee, add a new one
            if (compensationToUpdate == null)
            {
                _compensationRepository.Add(compensation);
                return compensation;
            }
            else
            {
                // Don't allow a new Compensation for an Employee that doesn't exist
                if (_employeeRepository.GetById(compensation.EmployeeId) == null)
                {
                    return null;
                }

                // Delete the existing Compensation
                _compensationRepository.Remove(compensationToUpdate);
                _compensationRepository.SaveAsync().Wait();
                // Once old Compensation is removed, Add in the new Compensation
                _compensationRepository.Add(compensation);
                _compensationRepository.SaveAsync().Wait();
                return compensation;
            }
        }
    }
}
