namespace FlowerEcommerce.Application.Handlers.Products.Commands.CreateProduct;

[EnableUnitOfWork]
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, TResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Đang tạo sản phẩm: {ProductName}", request.Name);
        try
        {
            // Kiểm tra trùng tên sản phẩm
            var existingProduct = await _unitOfWork.Repository<Product>()
                .AnyAsync(filters: [p => p.Name == request.Name.Trim()]);

            if (existingProduct)
            {
                return TResult.Failure(MessageKey.ProductAlreadyExists, ErrorCodes.ALREADY_EXISTS);
            }

            // Kiểm tra trùng tên danh mục
            var existingCategory = await _unitOfWork.Repository<Category>()
                .AnyAsync(filters: [c => c.Id == request.CategoryId]);

            if (existingCategory)
            {
                return TResult.Failure(MessageKey.CategoryNotFound, ErrorCodes.NOT_FOUND);
            }

            // Khởi tạo Entity bằng Constructor
            var product = new Product
            {
                Name = request.Name.Trim(),
                Description = request.Description,
                Price = request.Price,
                CategoryId = request.CategoryId
            };

            // Lưu vào Database
            await _unitOfWork.Repository<Product>().InsertAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Đã tạo thành công sản phẩm {ProductId}", product.Id);

            return TResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo sản phẩm");
            return TResult.Failure(MessageKey.InternalError, ErrorCodes.SERVER_ERROR);
        }
    }
}
