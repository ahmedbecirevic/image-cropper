using ImageCropper.Api.Dtos;
using ImageCropper.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageCropper.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImageController(IImageService imageService, ILogger<ImageController> logger)
    : ControllerBase
{
    [HttpPost("preview")]
    public async Task<IActionResult> Preview([FromForm] ImagePreviewRequestDto request)
    {
        try
        {
            var results = await imageService.GeneratePreviewsAsync(request);
            return Ok(results);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in Preview endpoint");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromForm] ImageCropRequestDto request, [FromQuery] int? configId = null)
    {
        try
        {
            var results = await imageService.GenerateImagesAsync(request, configId);
            return Ok(results);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in Generate endpoint");
            return StatusCode(500, "Internal server error");
        }
    }
}