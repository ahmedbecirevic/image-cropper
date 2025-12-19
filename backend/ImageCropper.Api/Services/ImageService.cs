using ImageCropper.Api.Dtos;
using ImageCropper.Api.Models;

namespace ImageCropper.Api.Services;

public class ImageService(
    IImageProcessingService imageProcessingService,
    IConfigurationService configurationService,
    ILogger<ImageService> logger)
    : IImageService
{
    public async Task<IEnumerable<ImageCropResponseDto>> GeneratePreviewsAsync(ImagePreviewRequestDto request)
    {
        var modelRequest = request.ToModel();
        ValidateImageRequest(modelRequest.Image);
        ValidatePreviewCropCoordinates(modelRequest.CropCoordinates);

        var results = new List<ImageCropResponse>();

        foreach (var coordinates in modelRequest.CropCoordinates)
            try
            {
                var croppedImageData =
                    await imageProcessingService.CropImagePreviewAsync(modelRequest.Image, coordinates);

                results.Add(new ImageCropResponse
                {
                    Coordinates = coordinates,
                    ImageData = Convert.ToBase64String(croppedImageData),
                    ContentType = "image/png"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing crop coordinates: {Coordinates}", coordinates);
                results.Add(new ImageCropResponse
                {
                    Coordinates = coordinates,
                    Error = ex.Message
                });
            }

        return results.Select(r => r.ToDto());
    }

    public async Task<IEnumerable<ImageCropResponseDto>> GenerateImagesAsync(ImageCropRequestDto request,
        int? configId = null)
    {
        var modelRequest = request.ToModel();
        ValidateImageRequest(modelRequest.Image);
        ValidateGenerationCropCoordinates(modelRequest.CropCoordinates);

        // Get configuration for logo overlay if configId is provided
        ConfigurationModel? configuration = null;
        if (configId.HasValue)
        {
            configuration = await configurationService.GetConfigurationModelByIdAsync(configId.Value);
            if (configuration == null) throw new KeyNotFoundException($"Configuration with ID {configId} not found");
        }

        var results = new List<ImageCropResponse>();

        foreach (var coordinates in modelRequest.CropCoordinates)
            try
            {
                var croppedImageData = await imageProcessingService.CropImageAsync(modelRequest.Image, coordinates);

                // Apply logo overlay if configuration is provided
                if (configuration != null && configuration.LogoImageData != null)
                    croppedImageData = await imageProcessingService.ApplyLogoOverlayAsync(
                        croppedImageData,
                        configuration.LogoImageData,
                        configuration.LogoPosition,
                        configuration.ScaleDown
                    );

                results.Add(new ImageCropResponse
                {
                    Coordinates = coordinates,
                    ImageData = Convert.ToBase64String(croppedImageData),
                    ContentType = "image/png",
                    HasLogoOverlay = configuration?.LogoImageData != null
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing crop coordinates: {Coordinates}", coordinates);
                results.Add(new ImageCropResponse
                {
                    Coordinates = coordinates,
                    Error = ex.Message
                });
            }

        return results.Select(r => r.ToDto());
    }

    private static void ValidateImageRequest(IFormFile? image)
    {
        if (image == null || image.Length == 0) throw new ArgumentException("No image provided");

        if (!image.ContentType.StartsWith("image/png", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Only PNG images are supported");
    }

    private static void ValidatePreviewCropCoordinates(ICollection<CropCoordinates> cropCoordinates)
    {
        if (cropCoordinates.Count < 3) throw new ArgumentException("At least 3 crop coordinates are required");
    }

    private static void ValidateGenerationCropCoordinates(ICollection<CropCoordinates> cropCoordinates)
    {
        if (cropCoordinates.Count == 0) throw new ArgumentException("At least one crop coordinate is required");
    }
}