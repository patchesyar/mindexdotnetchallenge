
using System.Net;
using System.Net.Http;
using System.Text;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class ReportingStructureControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void NoReportsEmployee_Returns_Zero()
        {

            // Arrange
            var employeeId = "b7839309-3348-463b-a7e3-5de1c168beb3";
            var expectedFirstName = "Paul";
            var expectedLastName = "McCartney";
            var expectedReportCount = 0;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/reportingstructure/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var structure = response.DeserializeContent<ReportingStructure>();
            Assert.AreEqual(expectedFirstName, structure.Employee.FirstName);
            Assert.AreEqual(expectedLastName, structure.Employee.LastName);
            Assert.AreEqual(expectedReportCount, structure.NumberOfReports);
        }

        [TestMethod]
        public void HasReportsEmployee_Returns_Count()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";
            var expectedReportCount = 4;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/reportingstructure/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var structure = response.DeserializeContent<ReportingStructure>();
            Assert.AreEqual(expectedFirstName, structure.Employee.FirstName);
            Assert.AreEqual(expectedLastName, structure.Employee.LastName);
            Assert.AreEqual(expectedReportCount, structure.NumberOfReports);
        }

        [TestMethod]
        public void FakeReportingStructure_Returns_NotFound()
        {
            // Arrange
            var employeeId = "Decidedly Not a Valid GUID";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/reportingstructure/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
