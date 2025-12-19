using ImageCropper.Api.Dtos;
using ImageCropper.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageCropper.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConfigController(IConfigurationService configurationService, ILogger<ConfigController> logger)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateConfiguration([FromForm] CreateConfigurationRequestDto request)
    {
        try
        {
            var configuration = await configurationService.CreateConfigurationAsync(request);
            return CreatedAtAction(nameof(GetConfiguration), new { id = configuration.Id }, configuration);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating configuration");
            return StatusCode(500, "An error occurred while creating the configuration");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateConfiguration(int id, [FromForm] UpdateConfigurationRequestDto request)
    {
        try
        {
            var configuration = await configurationService.UpdateConfigurationAsync(id, request);
            return Ok(configuration);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating configuration with ID: {ConfigurationId}", id);
            return StatusCode(500, "An error occurred while updating the configuration");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetConfiguration(int id)
    {
        try
        {
            var configuration = await configurationService.GetConfigurationByIdAsync(id);
            if (configuration == null) return NotFound($"Configuration with ID {id} not found");

            return Ok(configuration);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting configuration with ID: {ConfigurationId}", id);
            return StatusCode(500, "An error occurred while retrieving the configuration");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllConfigurations()
    {
        try
        {
            var configurations = await configurationService.GetAllConfigurationsAsync();
            return Ok(configurations);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all configurations");
            return StatusCode(500, "An error occurred while retrieving configurations");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConfiguration(int id)
    {
        try
        {
            var deleted = await configurationService.DeleteConfigurationAsync(id);
            if (!deleted) return NotFound($"Configuration with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting configuration with ID: {ConfigurationId}", id);
            return StatusCode(500, "An error occurred while deleting the configuration");
        }
    }
}