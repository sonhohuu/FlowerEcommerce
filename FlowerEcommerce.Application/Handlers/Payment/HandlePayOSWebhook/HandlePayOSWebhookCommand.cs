namespace FlowerEcommerce.Application.Handlers.Payment.HandlePayOSWebhook;

public record HandlePayOSWebhookCommand : IRequest<TResult<bool>>
{
    public WebhookData Data { get; init; } = default!;
}
