namespace FlowerEcommerce.Application.Handlers.Products.Commands.CreateProduct;

[EnableUnitOfWork]
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, TResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;
    private readonly ICloudinaryService _cloudinaryService;

    public CreateProductCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateProductCommandHandler> logger,
        ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<TResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingProduct = await _unitOfWork.Repository<Product>()
                .AnyAsync(predicate: p => p.Name == request.Name.Trim());

            if (existingProduct)
            {
                return TResult.Failure(MessageKey.ProductAlreadyExists, ErrorCodes.ALREADY_EXISTS);
            }

            var existingCategory = await _unitOfWork.Repository<Category>()
                .AnyAsync(predicate: c => c.Id == request.CategoryId);

            if (!existingCategory)
            {
                return TResult.Failure(MessageKey.CategoryNotFound, ErrorCodes.NOT_FOUND);
            }

            // ── Upload ảnh nếu có ──
            var productImages = new List<FileAttachment>();

            if (request.FileAttachMents is { Count: > 0 })
            {
                var uploadTasks = request.FileAttachMents
                    .Select(file => _cloudinaryService.UploadImageAsync(file, folder: "products"));

                var uploadResults = await Task.WhenAll(uploadTasks);

                var failedUploads = uploadResults.Where(r => !r.Success).ToList();
                if (failedUploads.Count > 0)
                {
                    _logger.LogWarning("Some images failed to upload: {Errors}",
                        string.Join(", ", failedUploads.Select(r => r.ErrorMessage)));
                    return TResult.Failure(MessageKey.ImageUploadFailed, ErrorCodes.BAD_REQUEST);
                }

                productImages = uploadResults
                    .Select((r, index) => new FileAttachment
                    {
                        PublicId = r.PublicId,
                        SecureUrl = r.SecureUrl,
                        Url = r.Url,
                        Format = r.Format,
                        Width = r.Width,
                        Height = r.Height,
                        Bytes = r.Bytes,
                        IsMain = index == 0,   // ảnh đầu tiên là ảnh chính
                        SortOrder = index,
                    })
                    .ToList();
            }

            var sizePrices = request.SizePrices?
                .Select(sp => new ProductSizePrices
                {
                    Label = sp.Label,
                    Price = sp.Price,
                })
                .ToList() ?? new List<ProductSizePrices>();

            var productDetail = new ProductDetail
            {
                Sku = request.Sku,
                Slug = StringUtils.GenerateSlug(request.Name),
                SizePrices = sizePrices,
            };

            var product = new Product
            {
                Name = request.Name.Trim(),
                Description = request.Description,
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                IsContactPrice = request.IsContactPrice,
                CategoryId = request.CategoryId,
                FileAttachments = productImages,
                ProductDetail = productDetail
            };

            _unitOfWork.Repository<Product>().Add(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created Product {ProductName} Successfully", product.Name);

            return TResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create Product Failed");
            return TResult.Failure(MessageKey.InternalError, ErrorCodes.SERVER_ERROR);
        }
    }
}
