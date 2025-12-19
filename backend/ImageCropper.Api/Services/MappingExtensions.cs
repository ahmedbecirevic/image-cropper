using ImageCropper.Api.Dtos;
using ImageCropper.Api.Models;

namespace ImageCropper.Api.Services;

public static class MappingExtensions
{
    public static ConfigurationDto ToDto(this ConfigurationModel model)
    {
        return new ConfigurationDto
        {
            Id = model.Id,
            ScaleDown = model.ScaleDown,
            LogoPosition = model.LogoPosition,
            HasLogoImage = model.LogoImageData != null,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }

    public static CreateConfigurationRequest ToModel(this CreateConfigurationRequestDto dto)
    {
        return new CreateConfigurationRequest
        {
            ScaleDown = dto.ScaleDown,
            LogoPosition = dto.LogoPosition,
            LogoImage = dto.LogoImage
        };
    }

    public static UpdateConfigurationRequest ToModel(this UpdateConfigurationRequestDto dto)
    {
        return new UpdateConfigurationRequest
        {
            ScaleDown = dto.ScaleDown,
            LogoPosition = dto.LogoPosition,
            LogoImage = dto.LogoImage
        };
    }

    public static CropCoordinates ToModel(this CropCoordinatesDto dto)
    {
        return new CropCoordinates
        {
            X = dto.X,
            Y = dto.Y,
            Width = dto.Width,
            Height = dto.Height
        };
    }

    public static CropCoordinatesDto ToDto(this CropCoordinates model)
    {
        return new CropCoordinatesDto
        {
            X = model.X,
            Y = model.Y,
            Width = model.Width,
            Height = model.Height
        };
    }

    public static ImageCropRequest ToModel(this ImageCropRequestDto dto)
    {
        return new ImageCropRequest
        {
            Image = dto.Image,
            CropCoordinates = dto.CropCoordinates.Select(c => c.ToModel()).ToList()
        };
    }

    public static ImagePreviewRequest ToModel(this ImagePreviewRequestDto dto)
    {
        return new ImagePreviewRequest
        {
            Image = dto.Image,
            CropCoordinates = dto.CropCoordinates.Select(c => c.ToModel()).ToList()
        };
    }

    public static ImageCropResponseDto ToDto(this ImageCropResponse model)
    {
        return new ImageCropResponseDto
        {
            Coordinates = model.Coordinates.ToDto(),
            ImageData = model.ImageData,
            ContentType = model.ContentType,
            HasLogoOverlay = model.HasLogoOverlay,
            Error = model.Error
        };
    }
}