namespace ImageCropper.Api.Models;

public class CropCoordinates
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class ImageCropRequest
{
    public IFormFile Image { get; set; } = null!;
    public List<CropCoordinates> CropCoordinates { get; set; } = new();
}

public class ImagePreviewRequest
{
    public IFormFile Image { get; set; } = null!;
    public List<CropCoordinates> CropCoordinates { get; set; } = new();
}

public class ImageCropResponse
{
    public CropCoordinates Coordinates { get; set; } = null!;
    public string? ImageData { get; set; }
    public string? ContentType { get; set; }
    public bool HasLogoOverlay { get; set; }
    public string? Error { get; set; }
}

public class ConfigurationModel
{
    public int Id { get; set; }
    public float ScaleDown { get; set; }
    public string LogoPosition { get; set; } = string.Empty;
    public byte[]? LogoImageData { get; set; }
    public string LogoImageContentType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class CreateConfigurationRequest
{
    public float ScaleDown { get; set; }
    public string LogoPosition { get; set; } = string.Empty;
    public IFormFile? LogoImage { get; set; }
}

public class UpdateConfigurationRequest
{
    public float ScaleDown { get; set; }
    public string LogoPosition { get; set; } = string.Empty;
    public IFormFile? LogoImage { get; set; }
}