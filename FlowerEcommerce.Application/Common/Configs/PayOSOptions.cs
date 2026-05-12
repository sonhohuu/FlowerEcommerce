namespace FlowerEcommerce.Application.Common.Configs;

public class PayOSOptions
{
    public const string SectionName = "PayOS";
    public string ClientId { get; init; } = default!;
    public string ApiKey { get; init; } = default!;
    public string ChecksumKey { get; init; } = default!;
    public string ReturnUrl { get; init; } = default!;
    public string CancelUrl { get; init; } = default!;
}
