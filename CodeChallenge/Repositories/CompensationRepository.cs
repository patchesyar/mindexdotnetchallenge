using CodeChallenge.Data;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<ICompensationRepository> _logger;

        public CompensationRepository(ILogger<ICompensationRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }
        // Returns the Compensation for the Employee with the given id
        public Compensation GetById(string id)
        {
            return _employeeContext.Compensations.FirstOrDefault(c => c.EmployeeId == id); ;
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        // Adds a Compensation to the Database
        public Compensation Add(Compensation compensation)
        {
            _employeeContext.Compensations.Add(compensation);
            return compensation;
        }
        // Removes a Compensation from the Database
        // Note: Only to be used in conjuction with add for Upserting
        // Requirements do not list a need for Deleting alone
        public Compensation Remove(Compensation compensation)
        {
            _employeeContext.Compensations.Remove(compensation);
            return compensation;
        }
    }
}
