namespace FlowerEcommerce.Application.Handlers.Products.Commands.UpdateProduct;

[EnableUnitOfWork]
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, TResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProductCommandHandler> _logger;
    private readonly ICloudinaryService _cloudinaryService;

    public UpdateProductCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateProductCommandHandler> logger,
        ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<TResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingProduct = await _unitOfWork.Repository<Product>()
                .FirstOrDefaultAsync(
                predicate:x => x.Id == request.Id,
                includes: [ nameof(Product.FileAttachments), 
                            nameof(Product.ProductDetail),
                            $"{nameof(Product.ProductDetail)}.{nameof(ProductDetail.SizePrices)}"],
                cancellationToken: cancellationToken);

            if (existingProduct == null)
            {
                return TResult.Failure(MessageKey.ProductNotFound, ErrorCodes.NOT_FOUND);
            }

            if (request.CategoryId.HasValue)
            {
                var existingCategory = await _unitOfWork.Repository<Category>()
                    .AnyAsync(c => c.Id == request.CategoryId.Value);

                if (!existingCategory)
                    return TResult.Failure(MessageKey.CategoryNotFound, ErrorCodes.NOT_FOUND);
            }

            request.Adapt(existingProduct);

            // ── Slug: re-generate nếu Name thay đổi ─────────────
            if (request.Name is not null && existingProduct.ProductDetail is not null)
                existingProduct.ProductDetail.Slug = StringUtils.GenerateSlug(request.Name);

            // ── SizePrices: Adapt từng item ──────────────────────
            if (request.SizePrices is { Count: > 0 } && existingProduct.ProductDetail is not null)
            {
                existingProduct.ProductDetail.SizePrices = request.SizePrices
                    .Adapt<List<ProductSizePrices>>();
            }

            // ── Xử lý ảnh nếu có upload mới ─────────────────────
            if (request.FileAttachMents is { Count: > 0 })
            {
                // 1. Upload ảnh mới trước
                var uploadTasks = request.FileAttachMents
                    .Select(f => _cloudinaryService.UploadImageAsync(f, folder: "products"));
                var uploadResults = await Task.WhenAll(uploadTasks);

                var failed = uploadResults.Where(r => !r.Success).ToList();
                if (failed.Count > 0)
                    return TResult.Failure(MessageKey.ImageUploadFailed, ErrorCodes.BAD_REQUEST);

                // 2. Xoá ảnh cũ trên Cloudinary
                var deleteTasks = existingProduct.FileAttachments
                    .Select(img => _cloudinaryService.DeleteAsync(img.PublicId));
                await Task.WhenAll(deleteTasks);

                // 3. Replace danh sách ảnh trong DB
                existingProduct.FileAttachments = uploadResults
                    .Select((r, index) => new FileAttachment
                    {
                        PublicId = r.PublicId,
                        SecureUrl = r.SecureUrl,
                        Url = r.Url,
                        Format = r.Format,
                        Width = r.Width,
                        Height = r.Height,
                        Bytes = r.Bytes,
                        IsMain = index == 0,
                        SortOrder = index
                    })
                    .ToList();
            }

            _unitOfWork.Repository<Product>().Update(existingProduct);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated Product {ProductName} Successfully", existingProduct.Name);

            return TResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Update Product Failed");
            return TResult.Failure(MessageKey.InternalError, ErrorCodes.SERVER_ERROR);
        }
    }
}
