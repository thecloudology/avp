using Microsoft.AspNetCore.Mvc;
using CSOTeamApp.Models;
using CSOTeamApp.Repositories;

namespace CSOTeamApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnvironmentVariableController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public EnvironmentVariableController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private EnvironmentVariableRepository GetRepository(string env)
        {
            var connectionString = _configuration.GetConnectionString(env ?? "Development");
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception($"Connection string for environment '{env}' not found.");
            return new EnvironmentVariableRepository(connectionString);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string env = "Development")
        {
            var repository = GetRepository(env);
            var result = await repository.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("process/{processId}")]
        public async Task<IActionResult> GetByProcessId(string processId, [FromQuery] string env = "Development")
        {
            var repository = GetRepository(env);
            var result = await repository.GetByProcessIdAsync(processId);
            return Ok(result);
        }

        [HttpGet("variable/{processId}/{elementName}")]
        public async Task<IActionResult> GetByElementName(string processId, string elementName, [FromQuery] string env = "Development")
        {
            var repository = GetRepository(env);
            var result = await repository.GetByElementNameAsync(processId, elementName);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetTypes([FromQuery] string env = "Development")
        {
            var repository = GetRepository(env);
            var result = await repository.GetTypesAsync();
            return Ok(result);
        }

        [HttpGet("exists/{processId}")]
        public async Task<IActionResult> ProcessExists(string processId, [FromQuery] string env = "Development")
        {
            var repository = GetRepository(env);
            var result = await repository.ExistsAsync(processId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] EnvironmentVariable envVar, [FromQuery] string env = "Development")
        {
            var repository = GetRepository(env);
            await repository.AddAsync(envVar);
            return Ok("Environment variable added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] EnvironmentVariable envVar, [FromQuery] string env = "Development")
        {
            var repository = GetRepository(env);
            await repository.UpdateAsync(envVar);
            return Ok("Environment variable updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] string env = "Development")
        {
            var repository = GetRepository(env);
            await repository.DeleteAsync(id);
            return Ok("Environment variable deleted successfully.");
        }
    }
}
