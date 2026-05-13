namespace FlowerEcommerce.Application.Common.Constants;

public static class AppConstants
{
    public const ulong DefaultRefreshTokenExpiryTime = 2_764_800; // 32 days
    public const ulong DefaultAccessTokenExpiryTime = 86_400; // 1 day

    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;
    public const int MaxCommonLength = 64;

    public const string DbCsKey = "AzureDatabase";
    public const string DateFormat = "yyyy-MM-dd";
    public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
    public const string DateHumanFormat = "dd-MM-yyyy";
    public const string DateTimeHumanFormat = "dd-MM-yyyy HH:mm:ss";


    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = true
    };
}

