namespace FlowerEcommerce.Application.Interfaces.Services;
public class CloudinaryUploadResult
{
    public bool Success { get; init; }
    public string PublicId { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string SecureUrl { get; init; } = string.Empty;
    public string Format { get; init; } = string.Empty;
    public long Bytes { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
    public string? ErrorMessage { get; init; }
}

public interface ICloudinaryService
{
    Task<CloudinaryUploadResult> UploadImageAsync(IFormFile file, string? folder = null);

    Task<CloudinaryUploadResult> UploadFileAsync(IFormFile file, string? folder = null);

    Task<CloudinaryUploadResult> UploadFromBase64Async(string base64Data, string fileName, string? folder = null);

    Task<bool> DeleteAsync(string publicId, ResourceType resourceType = ResourceType.Image);
}
