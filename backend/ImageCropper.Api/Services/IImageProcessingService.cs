using ImageCropper.Api.Models;

namespace ImageCropper.Api.Services;

public interface IImageProcessingService
{
    Task<byte[]> CropImageAsync(IFormFile image, CropCoordinates coordinates);
    Task<byte[]> CropImagePreviewAsync(IFormFile image, CropCoordinates coordinates, float scaleDown = 0.05f);
    Task<byte[]> ApplyLogoOverlayAsync(byte[] imageData, byte[] logoData, string logoPosition, float scaleDown = 1.0f);
}