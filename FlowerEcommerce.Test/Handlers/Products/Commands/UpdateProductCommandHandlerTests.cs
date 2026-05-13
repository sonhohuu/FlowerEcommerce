using FlowerEcommerce.Application.Common.Utils;
using FlowerEcommerce.Application.Handlers.Products.Commands.CreateProduct;
using FlowerEcommerce.Application.Handlers.Products.Commands.UpdateProduct;

namespace FlowerEcommerce.Test.Handlers.Products.Commands;

public class UpdateProductCommandHandlerTests : TestBase
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ICloudinaryService> _cloudinaryService;
    private readonly ILogger<UpdateProductCommandHandler> _logger;
    private readonly UpdateProductCommandHandler _handler;

    private readonly Mock<IBaseRepository<Product>> _productRepo;
    private readonly Mock<IBaseRepository<Category>> _categoryRepo;
    private readonly Mock<IBaseRepository<FileAttachment>> _fileRepo;

    public UpdateProductCommandHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _cloudinaryService = new Mock<ICloudinaryService>();
        _logger = NullLogger<UpdateProductCommandHandler>.Instance;

        _productRepo = new Mock<IBaseRepository<Product>>();
        _categoryRepo = new Mock<IBaseRepository<Category>>();
        _fileRepo = new Mock<IBaseRepository<FileAttachment>>();

        _unitOfWork.Setup(u => u.Repository<Product>()).Returns(_productRepo.Object);
        _unitOfWork.Setup(u => u.Repository<Category>()).Returns(_categoryRepo.Object);
        _unitOfWork.Setup(u => u.Repository<FileAttachment>()).Returns(_fileRepo.Object);

        _handler = new UpdateProductCommandHandler(
            _unitOfWork.Object,
            _logger,
            _cloudinaryService.Object);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private UpdateProductCommand BuildCommand(
        ulong? productId = null,
        ulong? categoryId = null,
        List<IFormFile>? files = null) => new UpdateProductCommand
        {
            Id = productId ?? 1,
            Name = "Updated Rose",
            CategoryId = categoryId,
            FileAttachMents = files,
            SizePrices = new List<SizePriceDto>
        {
            new SizePriceDto { Label = "M", Price = 100_000 }
        }
        };

    // shorthand để tránh lặp lại setup dài
    private void SetupProductRepo(Product? returns) =>
        _productRepo
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<List<string>>(),
                It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(returns);

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProductNotFound()
    {
        SetupProductRepo(null);

        var result = await _handler.Handle(BuildCommand(), default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.NOT_FOUND);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CategoryNotFound()
    {
        SetupProductRepo(CreateFakeProduct());
        _categoryRepo
            .Setup(r => r.AnyAsync(
                It.IsAny<Expression<Func<Category, bool>>>(),
                default))
            .ReturnsAsync(false);

        var result = await _handler.Handle(BuildCommand(categoryId: 99), default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.NOT_FOUND);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidRequest_NoFiles()
    {
        var product = CreateFakeProduct();
        SetupProductRepo(product);

        var result = await _handler.Handle(BuildCommand(), default);

        result.IsSuccess.Should().BeTrue();
        _productRepo.Verify(r => r.Update(product), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_DeleteOldImages_And_UploadNew_When_FilesProvided()
    {
        var product = CreateFakeProduct();
        var files = new List<IFormFile> { CreateFakeFormFile("new.jpg") };

        SetupProductRepo(product);
        _cloudinaryService
            .Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync(SuccessUploadResult("products/new1"));
        _cloudinaryService
            .Setup(c => c.DeleteAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(BuildCommand(files: files), default);

        result.IsSuccess.Should().BeTrue();
        _cloudinaryService.Verify(c => c.DeleteAsync("products/abc123"), Times.Once);
        _fileRepo.Verify(r => r.RemoveRange(It.IsAny<IEnumerable<FileAttachment>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NewImageUploadFails()
    {
        var product = CreateFakeProduct();
        var files = new List<IFormFile> { CreateFakeFormFile() };

        SetupProductRepo(product);
        _cloudinaryService
            .Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync(FailedUploadResult());

        var result = await _handler.Handle(BuildCommand(files: files), default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.BAD_REQUEST);
        // Không được xoá ảnh cũ nếu upload mới fail
        _cloudinaryService.Verify(c => c.DeleteAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_RegenerateSlug_When_NameChanged()
    {
        var product = CreateFakeProduct();
        SetupProductRepo(product);

        var command = BuildCommand();
        command.Name = "Sunflower Basket";

        await _handler.Handle(command, default);

        product.ProductDetail!.Slug.Should().Be(StringUtils.GenerateSlug("Sunflower Basket"));
    }

    [Fact]
    public async Task Handle_Should_ReturnServerError_When_ExceptionThrown()
    {
        _productRepo
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<List<string>>(),
                It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Timeout"));

        var result = await _handler.Handle(BuildCommand(), default);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.SERVER_ERROR);
    }
}
