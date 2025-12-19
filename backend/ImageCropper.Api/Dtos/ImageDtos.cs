namespace ImageCropper.Api.Dtos;

public class ConfigurationDto
{
    public int Id { get; set; }
    public float ScaleDown { get; set; }
    public string LogoPosition { get; set; } = string.Empty;
    public bool HasLogoImage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateConfigurationRequestDto
{
    public float ScaleDown { get; set; }
    public string LogoPosition { get; set; } = string.Empty;
    public IFormFile? LogoImage { get; set; }
}

public class UpdateConfigurationRequestDto
{
    public float ScaleDown { get; set; }
    public string LogoPosition { get; set; } = string.Empty;
    public IFormFile? LogoImage { get; set; }
}

public class CropCoordinatesDto
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class ImageCropRequestDto
{
    public IFormFile Image { get; set; } = null!;
    public List<CropCoordinatesDto> CropCoordinates { get; set; } = new();
}

public class ImagePreviewRequestDto
{
    public IFormFile Image { get; set; } = null!;
    public List<CropCoordinatesDto> CropCoordinates { get; set; } = new();
}

public class ImageCropResponseDto
{
    public CropCoordinatesDto Coordinates { get; set; } = null!;
    public string? ImageData { get; set; }
    public string? ContentType { get; set; }
    public bool HasLogoOverlay { get; set; }
    public string? Error { get; set; }
}