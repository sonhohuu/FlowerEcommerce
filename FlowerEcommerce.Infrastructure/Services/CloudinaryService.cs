using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace FlowerEcommerce.Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<CloudinaryService> _logger;

    public CloudinaryService(
        IOptions<CloudinarySettings> options,
        ILogger<CloudinaryService> logger)
    {
        var cfg = options.Value;

        var account = new Account(cfg.CloudName, cfg.ApiKey, cfg.ApiSecret);
        _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
        _logger = logger;
    }

    // ── Upload image ──────────────────────────────────────────────────────────

    public async Task<CloudinaryUploadResult> UploadImageAsync(IFormFile file, string? folder = null)
    {
        try
        {
            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error is not null)
            {
                _logger.LogError("Cloudinary upload error: {Msg}", result.Error.Message);
                return Fail(result.Error.Message);
            }

            return MapImageResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while uploading image to Cloudinary");
            return Fail(ex.Message);
        }
    }

    // ── Upload raw file ───────────────────────────────────────────────────────

    public async Task<CloudinaryUploadResult> UploadFileAsync(IFormFile file, string? folder = null)
    {
        try
        {
            await using var stream = file.OpenReadStream();

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error is not null)
            {
                _logger.LogError("Cloudinary upload error: {Msg}", result.Error.Message);
                return Fail(result.Error.Message);
            }

            return new CloudinaryUploadResult
            {
                Success = true,
                PublicId = result.PublicId,
                Url = result.Url?.ToString() ?? string.Empty,
                SecureUrl = result.SecureUrl?.ToString() ?? string.Empty,
                Bytes = result.Bytes,
                Format = result.Format ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while uploading file to Cloudinary");
            return Fail(ex.Message);
        }
    }

    // ── Upload from base64 ────────────────────────────────────────────────────

    public async Task<CloudinaryUploadResult> UploadFromBase64Async(
        string base64Data, string fileName, string? folder = null)
    {
        try
        {
            // Accept both raw base64 and data-URI format
            var dataUri = base64Data.StartsWith("data:")
                ? base64Data
                : $"data:image/png;base64,{base64Data}";

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, dataUri),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = true
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error is not null)
            {
                _logger.LogError("Cloudinary base64 upload error: {Msg}", result.Error.Message);
                return Fail(result.Error.Message);
            }

            return MapImageResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while uploading base64 to Cloudinary");
            return Fail(ex.Message);
        }
    }

    public async Task<bool> DeleteAsync(string publicId, Domain.Enums.ResourceType resourceType = Domain.Enums.ResourceType.Image)
    {
        try
        {
            var deleteParams = new DeletionParams(publicId)
            {
                ResourceType = MapResourceType(resourceType)
            };

            var result = await _cloudinary.DestroyAsync(deleteParams);
            var ok = result.Result == "ok";

            if (!ok)
                _logger.LogWarning("Cloudinary delete returned: {Result} for publicId={Id}", result.Result, publicId);

            return ok;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting resource {Id} from Cloudinary", publicId);
            return false;
        }
    }

    private static CloudinaryUploadResult MapImageResult(ImageUploadResult r) =>
    new()
    {
        Success = true,
        PublicId = r.PublicId,
        Url = r.Url?.ToString() ?? string.Empty,
        SecureUrl = r.SecureUrl?.ToString() ?? string.Empty,
        Format = r.Format ?? string.Empty,
        Bytes = r.Bytes,
        Width = r.Width,
        Height = r.Height
    };

    private static CloudinaryUploadResult Fail(string msg) =>
        new() { Success = false, ErrorMessage = msg };

    private static CloudinaryDotNet.Actions.ResourceType MapResourceType(Domain.Enums.ResourceType rt) =>
        rt switch
        {
            Domain.Enums.ResourceType.Video => CloudinaryDotNet.Actions.ResourceType.Video,
            Domain.Enums.ResourceType.Raw => CloudinaryDotNet.Actions.ResourceType.Raw,
            _ => CloudinaryDotNet.Actions.ResourceType.Image
        };
}
