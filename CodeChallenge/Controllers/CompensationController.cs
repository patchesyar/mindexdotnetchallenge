using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/compensation")]
    public class CompensationController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ICompensationService _compensationService;

        public CompensationController(ILogger<CompensationController> logger, ICompensationService compensationService)
        {
            _logger = logger;
            _compensationService = compensationService;
        }

        [HttpPut("{id}")]
        public IActionResult UpsertCompensation(String id, [FromBody] Compensation compensation)
        {
            _logger.LogDebug($"Received compensation create/update request for '{compensation.EmployeeId}'");
            // If the body has no id, add it from the url
            if (compensation.EmployeeId == null)
            {
                compensation.EmployeeId = id;
            }
            // Don't proceed if the body and url don't match
            if (id != compensation.EmployeeId)
            {
                return BadRequest();
            }

            Compensation response;
            try
            {
                response = _compensationService.Upsert(compensation);
            }
            catch (InvalidOperationException ex)
            {
                // adding a compensation for nonexistent employee will fail and
                // throw an exception, here responding with Not Found
                return NotFound();
            }
            // If we updated an existing record then give a No Content success code
            if (response == null)
            {
                return NoContent();
            }

            return CreatedAtRoute("getCompensationById", new { id = compensation.EmployeeId }, compensation);
        }

        [HttpGet("{id}", Name = "getCompensationById")]
        public IActionResult GetCompensationById(String id)
        {
            _logger.LogDebug($"Received compensation GET request for '{id}'");

            var compensation = _compensationService.GetById(id);

            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }
    }
}
