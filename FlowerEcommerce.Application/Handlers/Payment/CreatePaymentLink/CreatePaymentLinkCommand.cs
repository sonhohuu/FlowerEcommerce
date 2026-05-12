namespace FlowerEcommerce.Application.Handlers.Payment.CreatePaymentLink;

public record CreatePaymentLinkCommand : IRequest<TResult<CreatePaymentLinkResult>>
{
    public ulong OrderId { get; init; }
}

public record CreatePaymentLinkResult(
    string CheckoutUrl,
    string QrCode,
    long OrderCode
);
