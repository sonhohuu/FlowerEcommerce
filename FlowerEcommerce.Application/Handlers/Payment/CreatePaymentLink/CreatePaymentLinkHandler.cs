using FlowerEcommerce.Application.Common.Configs;
using Microsoft.Extensions.Options;
using System.Diagnostics.Contracts;

namespace FlowerEcommerce.Application.Handlers.Payment.CreatePaymentLink;

public class CreatePaymentLinkHandler : IRequestHandler<CreatePaymentLinkCommand, TResult<CreatePaymentLinkResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPayOSService _payOS;
    private readonly PayOSOptions _options;

    public CreatePaymentLinkHandler(
        IUnitOfWork unitOfWork,
        IPayOSService payOS,
        IOptions<PayOSOptions> options)
    {
        _unitOfWork = unitOfWork;
        _payOS = payOS;
        _options = options.Value;
    }

    public async Task<TResult<CreatePaymentLinkResult>> Handle(
        CreatePaymentLinkCommand command,
        CancellationToken ct)
    {
        var order = await _unitOfWork.Repository<Order>().FirstOrDefaultAsync(
            predicate: o => o.Id == command.OrderId,
            includes: [
                nameof(Order.Items),
                $"{nameof(Order.Items)}.{nameof(OrderItem.Product)}"]
                      ,
            cancellationToken: ct);

        if (order == null) 
        {
            return TResult<CreatePaymentLinkResult>.Failure("Order not found");
        }
            
        var result = await _payOS.CreatePaymentLinkAsync(new CreatePaymentRequest(
            OrderCode: (long)order.Id,
            Amount: (int)order.TotalAmount,
            Description: $"Thanh toan {order.OrderCode}",
            BuyerName: order.CustomerName,
            BuyerPhone: order.PhoneNumber,
            Items: order.Items
                .Select(d => new PaymentItem
                {
                    Name = d.Product.Name,
                    Quantity = d.Quantity,
                    Price = (int)d.Price
                })
                .ToList(),
            ReturnUrl: _options.ReturnUrl,
            CancelUrl: _options.CancelUrl
        ));

        return TResult<CreatePaymentLinkResult>.Success(new CreatePaymentLinkResult(
            CheckoutUrl: result.CheckoutUrl,
            QrCode: result.QrCode,
            OrderCode: result.OrderCode
        ));
    }
}
