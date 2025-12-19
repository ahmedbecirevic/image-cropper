using ImageCropper.Api.Dtos;
using ImageCropper.Api.Models;

namespace ImageCropper.Api.Services;

public interface IConfigurationService
{
    Task<ConfigurationDto> CreateConfigurationAsync(CreateConfigurationRequestDto request);
    Task<ConfigurationDto> UpdateConfigurationAsync(int id, UpdateConfigurationRequestDto request);
    Task<ConfigurationDto?> GetConfigurationByIdAsync(int id);
    Task<IEnumerable<ConfigurationDto>> GetAllConfigurationsAsync();
    Task<bool> DeleteConfigurationAsync(int id);

    // Internal method for services that need the full model with logo data
    Task<ConfigurationModel?> GetConfigurationModelByIdAsync(int id);
}