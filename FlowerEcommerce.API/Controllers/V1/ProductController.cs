using FlowerEcommerce.Application.Handlers.Products.Commands.CreateProduct;
using FlowerEcommerce.Application.Handlers.Products.Commands.DeleteProduct;
using FlowerEcommerce.Application.Handlers.Products.Commands.UpdateProduct;
using FlowerEcommerce.Application.Handlers.Products.Queries.GetProductById;
using FlowerEcommerce.Application.Handlers.Products.Queries.GetProducts;

namespace FlowerEcommerce.API.Controllers;

[ApiVersion("1.0")]
[Route("api/[controller]")]
public class ProductController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        [FromForm] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(null))
            : HandleResult(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProduct(
        [FromBody] UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(null))
            : HandleResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(
        [FromRoute] ulong id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand { Id = id };
        var result = await Mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(null))
            : HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(
        [FromRoute] ulong id,
        CancellationToken cancellationToken)
    {
        var test = id;
        var query = new GetProductByIdQuery { Id = id };
        var result = await Mediator.Send(query, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<ProductDto>.Ok(result.Data))
            : HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        CancellationToken cancellationToken)
    {
        var query = new GetProductsQuery();
        var result = await Mediator.Send(query, cancellationToken);
        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(result.Data))
            : HandleResult(result);
    }

}
