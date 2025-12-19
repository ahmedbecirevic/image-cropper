using ImageCropper.Api.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageCropper.Api.Services;

public class ImageProcessingService : IImageProcessingService
{
    public async Task<byte[]> CropImageAsync(IFormFile image, CropCoordinates coordinates)
    {
        using var stream = new MemoryStream();
        await image.CopyToAsync(stream);
        stream.Position = 0;

        using var sourceImage = await Image.LoadAsync<Rgba32>(stream);

        // Validate crop coordinates
        var cropRect = new Rectangle(
            coordinates.X,
            coordinates.Y,
            Math.Min(coordinates.Width, sourceImage.Width - coordinates.X),
            Math.Min(coordinates.Height, sourceImage.Height - coordinates.Y)
        );

        if (cropRect.X < 0 || cropRect.Y < 0 ||
            cropRect.X + cropRect.Width > sourceImage.Width ||
            cropRect.Y + cropRect.Height > sourceImage.Height)
            throw new ArgumentException("Crop coordinates are outside image boundaries");

        sourceImage.Mutate(x => x.Crop(cropRect));

        var outputStream = new MemoryStream();
        await sourceImage.SaveAsync(outputStream, new PngEncoder());
        return outputStream.ToArray();
    }

    public async Task<byte[]> CropImagePreviewAsync(IFormFile image, CropCoordinates coordinates,
        float scaleDown = 0.05f)
    {
        var croppedImage = await CropImageAsync(image, coordinates);

        using var croppedStream = new MemoryStream(croppedImage);
        using var sourceImage = await Image.LoadAsync<Rgba32>(croppedStream);

        var newWidth = (int)(sourceImage.Width * scaleDown);
        var newHeight = (int)(sourceImage.Height * scaleDown);

        sourceImage.Mutate(x => x.Resize(newWidth, newHeight));

        var outputStream = new MemoryStream();
        await sourceImage.SaveAsync(outputStream, new PngEncoder());
        return outputStream.ToArray();
    }

    public async Task<byte[]> ApplyLogoOverlayAsync(byte[] imageData, byte[] logoData, string logoPosition,
        float scaleDown = 1.0f)
    {
        using var imageStream = new MemoryStream(imageData);
        using var logoStream = new MemoryStream(logoData);

        using var baseImage = await Image.LoadAsync<Rgba32>(imageStream);
        using var logoImage = await Image.LoadAsync<Rgba32>(logoStream);

        // Scale the logo to be 10% of the base image width
        var logoWidth = (int)(baseImage.Width * 0.1);
        var logoHeight = logoImage.Height * logoWidth / logoImage.Width;

        logoImage.Mutate(x => x.Resize(logoWidth, logoHeight));

        // Calculate position based on logoPosition parameter
        var position = logoPosition.ToLower() switch
        {
            "top-left" => new Point(10, 10),
            "top-right" => new Point(baseImage.Width - logoWidth - 10, 10),
            "bottom-left" => new Point(10, baseImage.Height - logoHeight - 10),
            "bottom-right" => new Point(baseImage.Width - logoWidth - 10, baseImage.Height - logoHeight - 10),
            "center" => new Point((baseImage.Width - logoWidth) / 2, (baseImage.Height - logoHeight) / 2),
            _ => new Point(10, 10) // Default to top-left
        };

        baseImage.Mutate(x => x.DrawImage(logoImage, position, 1f));

        // Apply scale down if requested
        if (scaleDown < 1.0f)
        {
            var newWidth = (int)(baseImage.Width * scaleDown);
            var newHeight = (int)(baseImage.Height * scaleDown);
            baseImage.Mutate(x => x.Resize(newWidth, newHeight));
        }

        var outputStream = new MemoryStream();
        await baseImage.SaveAsync(outputStream, new PngEncoder());
        return outputStream.ToArray();
    }
}