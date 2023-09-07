
using System;
using System.Net;
using System.Net.Http;
using System.Text;

using CodeChallenge.Models;
using CodeChallenge.Data;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
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
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            var employeeId = "b7839309-3348-463b-a7e3-5de1c168beb3"; // Paul
            var salary = 3.50m;
            var effectiveDate = DateTime.UtcNow;
            var compensation = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = salary,
                EffectiveDate = effectiveDate
            };
            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/compensation/{employeeId}",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = putRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var newCompensation = response.DeserializeContent<Compensation>();

            Assert.AreEqual(compensation.EmployeeId, newCompensation.EmployeeId);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
        }

        [TestMethod]
        public void CreateCompensationNoId_Returns_Created()
        {
            // Arrange
            var employeeId = "c0c2293d-16bd-4603-8e08-638a9d18b22c"; // George
            var salary = 3.50m;
            var effectiveDate = DateTime.UtcNow;
            var compensation = new Compensation()
            {
                Salary = salary,
                EffectiveDate = effectiveDate
            };
            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/compensation/{employeeId}",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = putRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var newCompensation = response.DeserializeContent<Compensation>();

            Assert.AreEqual(employeeId, newCompensation.EmployeeId);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
        }

        [TestMethod]
        public void CreateCompensationMismatchedIds_Returns_BadRequest()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f"; // John 
            var salary = 3.50m;
            var effectiveDate = DateTime.UtcNow;
            var compensation = new Compensation()
            {
                EmployeeId = "FakeData",
                Salary = salary,
                EffectiveDate = effectiveDate
            };
            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/compensation/{employeeId}",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = putRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void CreateCompensationNoEmployee_Returns_NotFound()
        {
            // Arrange
            var employeeId = "Balderdash";
            var salary = 3.50m;
            var effectiveDate = DateTime.UtcNow;
            var compensation = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = salary,
                EffectiveDate = effectiveDate
            };
            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/compensation/{employeeId}",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = putRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetCompensation_Returns_Ok()
        {
            // Arrange
            var employeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f"; // Ringo
            var salary = 3.50m;
            var effectiveDate = DateTime.UtcNow;
            var compensation = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = salary,
                EffectiveDate = effectiveDate
            };
            var requestContent = new JsonSerialization().ToJson(compensation);
            _httpClient.PutAsync($"api/compensation/{employeeId}",
                new StringContent(requestContent, Encoding.UTF8, "application/json")).Wait();

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var getResponse = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);
            var getCompensation = getResponse.DeserializeContent<Compensation>();
            Assert.AreEqual(compensation.EmployeeId, getCompensation.EmployeeId);
            Assert.AreEqual(compensation.Salary, getCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, getCompensation.EffectiveDate);
        }

        [TestMethod]
        public void GetCompensation_Returns_NotFound()
        {
            // Arrange
            var employeeId = "FakeGuid";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var getResponse = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [TestMethod]
        public void UpdateCompensation_returns_NoContent()
        {
            // Arrange - Create
            var employeeId = "62c1084e-6e34-4630-93fd-9153afb65309"; // Pete
            var salary = 3.50m;
            var effectiveDate = DateTime.UtcNow;
            var compensation = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = salary,
                EffectiveDate = effectiveDate
            };
            var requestContent = new JsonSerialization().ToJson(compensation);
            _httpClient.PutAsync($"api/compensation/{employeeId}",
                new StringContent(requestContent, Encoding.UTF8, "application/json")).Wait();

            // Arrange - Update
            var updatedSalary = 10000m;
            var updatedEffectiveDate = DateTime.UtcNow;
            var updatedCompensation = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = updatedSalary,
                EffectiveDate = updatedEffectiveDate
            };

            // Execute
            var updateRequestContent = new JsonSerialization().ToJson(updatedCompensation);
            var updateRequestTask = _httpClient.PutAsync($"api/compensation/{employeeId}",
                           new StringContent(updateRequestContent, Encoding.UTF8, "application/json"));
            var updateResponse = updateRequestTask.Result;

            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var getResponse = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, updateResponse.StatusCode);
            var getCompensation = getResponse.DeserializeContent<Compensation>();
            Assert.AreEqual(updatedCompensation.EmployeeId, getCompensation.EmployeeId);
            Assert.AreEqual(updatedCompensation.Salary, getCompensation.Salary);
            Assert.AreEqual(updatedCompensation.EffectiveDate, getCompensation.EffectiveDate);
        }
    }
}
