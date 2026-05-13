using FlowerEcommerce.Application.Handlers.Categories.Commands.DeleteCategory;
using FlowerEcommerce.Application.Handlers.Orders.Commands.CreateOrder;
using FlowerEcommerce.Application.Handlers.Orders.Commands.DeleteOrder;
using FlowerEcommerce.Application.Handlers.Orders.Commands.UpdateOrder;
using FlowerEcommerce.Application.Handlers.Orders.Queries.GetOrders;

namespace FlowerEcommerce.API.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/[controller]")]
public class OrderController : BaseController
{
    [Authorize(Policy = AppPolicy.AdminOrCustomer)]
    [HttpPost(Name = "CreateOrder")]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(result.Data))
            : HandleResult(result);
    }

    [Authorize(Policy = AppPolicy.AdminOnly)]
    [HttpPut("{id}", Name = "UpdateOrder")]
    public async Task<IActionResult> UpdateOrder(
        [FromBody] UpdateOrderCommand command,
        [FromRoute] ulong id,
        CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await Mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(null))
            : HandleResult(result);
    }

    [Authorize(Policy = AppPolicy.CustomerOnly)]
    [HttpDelete("{id}", Name = "DeleteOrder")]
    public async Task<IActionResult> DeleteOrder(
        [FromRoute] ulong id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteOrderCommand { Id = id };
        var result = await Mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(null))
            : HandleResult(result);
    }

    [Authorize(Policy = AppPolicy.AdminOrCustomer)]
    [HttpGet(Name = "GetOrders")]
    public async Task<IActionResult> GetOrders([FromQuery] GetOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(result.Data))
            : HandleResult(result);
    }
}
