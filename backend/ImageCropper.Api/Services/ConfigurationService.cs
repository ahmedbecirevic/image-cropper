using ImageCropper.Api.Data;
using ImageCropper.Api.Dtos;
using ImageCropper.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageCropper.Api.Services;

public class ConfigurationService(ApplicationDbContext context, ILogger<ConfigurationService> logger)
    : IConfigurationService
{
    public async Task<ConfigurationDto> CreateConfigurationAsync(CreateConfigurationRequestDto request)
    {
        var modelRequest = request.ToModel();

        // Validate scale down
        if (modelRequest.ScaleDown <= 0 || modelRequest.ScaleDown > 0.25f)
            throw new ArgumentException("ScaleDown must be between 0.01 and 0.25");

        // Validate logo position
        var validPositions = new[] { "top-left", "top-right", "bottom-left", "bottom-right" };
        if (!validPositions.Contains(modelRequest.LogoPosition.ToLower()))
            throw new ArgumentException("LogoPosition must be one of: top-left, top-right, bottom-left, bottom-right");

        byte[]? logoImageData = null;
        var logoImageContentType = string.Empty;

        if (modelRequest.LogoImage != null && modelRequest.LogoImage.Length > 0)
        {
            if (!modelRequest.LogoImage.ContentType.StartsWith("image/png", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Logo image must be a PNG file");

            using var logoStream = new MemoryStream();
            await modelRequest.LogoImage.CopyToAsync(logoStream);
            logoImageData = logoStream.ToArray();
            logoImageContentType = modelRequest.LogoImage.ContentType;
        }

        var configuration = new ConfigurationModel
        {
            ScaleDown = modelRequest.ScaleDown,
            LogoPosition = modelRequest.LogoPosition.ToLower(),
            LogoImageData = logoImageData,
            LogoImageContentType = logoImageContentType,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Configurations.Add(configuration);
        await context.SaveChangesAsync();

        logger.LogInformation("Configuration created with ID: {ConfigurationId}", configuration.Id);
        return configuration.ToDto();
    }

    public async Task<ConfigurationDto> UpdateConfigurationAsync(int id, UpdateConfigurationRequestDto request)
    {
        var configuration = await context.Configurations.FindAsync(id);
        if (configuration == null) throw new KeyNotFoundException($"Configuration with ID {id} not found");

        var modelRequest = request.ToModel();

        // Validate scale down
        if (modelRequest.ScaleDown <= 0 || modelRequest.ScaleDown > 0.25f)
            throw new ArgumentException("ScaleDown must be between 0.01 and 0.25");

        // Validate logo position
        var validPositions = new[] { "top-left", "top-right", "bottom-left", "bottom-right" };
        if (!validPositions.Contains(modelRequest.LogoPosition.ToLower()))
            throw new ArgumentException("LogoPosition must be one of: top-left, top-right, bottom-left, bottom-right");

        // Update basic properties
        configuration.ScaleDown = modelRequest.ScaleDown;
        configuration.LogoPosition = modelRequest.LogoPosition.ToLower();
        configuration.UpdatedAt = DateTime.UtcNow;

        // Update logo image if provided
        if (modelRequest.LogoImage != null && modelRequest.LogoImage.Length > 0)
        {
            if (!modelRequest.LogoImage.ContentType.StartsWith("image/png", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Logo image must be a PNG file");

            using var logoStream = new MemoryStream();
            await modelRequest.LogoImage.CopyToAsync(logoStream);
            configuration.LogoImageData = logoStream.ToArray();
            configuration.LogoImageContentType = modelRequest.LogoImage.ContentType;
        }

        await context.SaveChangesAsync();

        logger.LogInformation("Configuration updated with ID: {ConfigurationId}", configuration.Id);
        return configuration.ToDto();
    }

    public async Task<ConfigurationDto?> GetConfigurationByIdAsync(int id)
    {
        var configuration = await context.Configurations.FindAsync(id);
        return configuration?.ToDto();
    }

    public async Task<IEnumerable<ConfigurationDto>> GetAllConfigurationsAsync()
    {
        var configurations = await context.Configurations.ToListAsync();
        return configurations.Select(c => c.ToDto());
    }

    public async Task<bool> DeleteConfigurationAsync(int id)
    {
        var configuration = await context.Configurations.FindAsync(id);
        if (configuration == null) return false;

        context.Configurations.Remove(configuration);
        await context.SaveChangesAsync();

        logger.LogInformation("Configuration deleted with ID: {ConfigurationId}", id);
        return true;
    }

    public async Task<ConfigurationModel?> GetConfigurationModelByIdAsync(int id)
    {
        return await context.Configurations.FindAsync(id);
    }
}