using ImageCropper.Api.Dtos;

namespace ImageCropper.Api.Services;

public interface IImageService
{
    Task<IEnumerable<ImageCropResponseDto>> GeneratePreviewsAsync(ImagePreviewRequestDto request);
    Task<IEnumerable<ImageCropResponseDto>> GenerateImagesAsync(ImageCropRequestDto request, int? configId = null);
}