using FlowerEcommerce.Application.Handlers.Payment.HandlePayOSWebhook;

namespace FlowerEcommerce.API.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/[controller]")]
public class PaymentController : BaseController
{
    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] WebhookData data, CancellationToken cancellationToken)
    {
        var ok = await Mediator.Send(new HandlePayOSWebhookCommand { Data = data }, cancellationToken);
        // PayOS yêu cầu luôn trả 200, dù thành công hay thất bại
        return Ok(new { success = ok });
    }
}
