using FlowerEcommerce.Application.Handlers.Products.Commands.CreateProduct;

namespace FlowerEcommerce.Test.Handlers.Products.Commands;

public class CreateProductCommandHandlerTests : TestBase
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ICloudinaryService> _cloudinaryService;
    private readonly ILogger<CreateProductCommandHandler> _logger;
    private readonly CreateProductCommandHandler _handler;

    // ── Shared repo mocks ──────────────────────────────────────────
    private readonly Mock<IBaseRepository<Product>> _productRepo;
    private readonly Mock<IBaseRepository<Category>> _categoryRepo;

    public CreateProductCommandHandlerTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _cloudinaryService = new Mock<ICloudinaryService>();
        _logger = NullLogger<CreateProductCommandHandler>.Instance;

        _productRepo = new Mock<IBaseRepository<Product>>();
        _categoryRepo = new Mock<IBaseRepository<Category>>();

        _unitOfWork.Setup(u => u.Repository<Product>()).Returns(_productRepo.Object);
        _unitOfWork.Setup(u => u.Repository<Category>()).Returns(_categoryRepo.Object);

        _handler = new CreateProductCommandHandler(
            _unitOfWork.Object,
            _logger,
            _cloudinaryService.Object);
    }

    // ── Helpers ────────────────────────────────────────────────────
    private CreateProductCommand BuildCommand(
        string name = "Rose Bouquet",
        List<IFormFile>? files = null) => new CreateProductCommand
        {
            Name = name,
            Description = "Fresh roses",
            Price = 100_000,
            OriginalPrice = 120_000,
            IsContactPrice = false,
            CategoryId = 1,
            Sku = "SKU-001",
            FileAttachMents = files,
            SizePrices = new List<SizePriceDto>
        {
            new SizePriceDto { Label = "S", Price = 80_000 }
        }
        };

    // ── Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProductNameAlreadyExists()
    {
        // Arrange
        _productRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), default))
                    .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.ALREADY_EXISTS);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CategoryNotFound()
    {
        // Arrange
        _productRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), default))
                    .ReturnsAsync(false);
        _categoryRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(), default))
                     .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.NOT_FOUND);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_NoFiles()
    {
        // Arrange
        _productRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), default))
                    .ReturnsAsync(false);
        _categoryRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(), default))
                     .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _productRepo.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_And_UploadImages_When_FilesProvided()
    {
        // Arrange
        var files = new List<IFormFile> { CreateFakeFormFile("rose1.jpg"), CreateFakeFormFile("rose2.jpg") };
        var command = BuildCommand(files: files);

        _productRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), default))
                    .ReturnsAsync(false);
        _categoryRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(), default))
                     .ReturnsAsync(true);
        _cloudinaryService
            .Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync(SuccessUploadResult());

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _cloudinaryService.Verify(
            c => c.UploadImageAsync(It.IsAny<IFormFile>(), "products"),
            Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ImageUploadFails()
    {
        // Arrange
        var files = new List<IFormFile> { CreateFakeFormFile() };
        var command = BuildCommand(files: files);

        _productRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), default))
                    .ReturnsAsync(false);
        _categoryRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(), default))
                     .ReturnsAsync(true);
        _cloudinaryService
            .Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync(FailedUploadResult());

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.BAD_REQUEST);
    }

    [Fact]
    public async Task Handle_Should_SetFirstImage_AsMain()
    {
        // Arrange
        Product? savedProduct = null;
        var files = new List<IFormFile> { CreateFakeFormFile("img1.jpg"), CreateFakeFormFile("img2.jpg") };
        var command = BuildCommand(files: files);

        _productRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), default))
                    .ReturnsAsync(false);
        _categoryRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(), default))
                     .ReturnsAsync(true);
        _cloudinaryService
            .Setup(c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync(SuccessUploadResult());
        _productRepo
            .Setup(r => r.Add(It.IsAny<Product>()))
            .Callback<Product>(p => savedProduct = p);

        // Act
        await _handler.Handle(command, default);

        // Assert
        savedProduct.Should().NotBeNull();
        savedProduct!.FileAttachments.First().IsMain.Should().BeTrue();
        savedProduct.FileAttachments.Skip(1).All(f => !f.IsMain).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnServerError_When_ExceptionThrown()
    {
        // Arrange
        _productRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), default))
                    .ThrowsAsync(new Exception("DB connection lost"));

        // Act
        var result = await _handler.Handle(BuildCommand(), default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.SERVER_ERROR);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_SizePricesIsNull()
    {
        var command = BuildCommand();
        command.SizePrices = null; // ← cover null-coalescing branch

        _productRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), default))
            .ReturnsAsync(false);
        _categoryRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(), default))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, default);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_FileAttachmentsIsEmpty()
    {
        var command = BuildCommand(files: new List<IFormFile>()); // ← Count = 0

        _productRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), default))
            .ReturnsAsync(false);
        _categoryRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(), default))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, default);
        result.IsSuccess.Should().BeTrue();
        _cloudinaryService.Verify(
            c => c.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>()),
            Times.Never); // không gọi upload
    }
}
