namespace FlowerEcommerce.Test.Common;

public abstract class TestBase
{
    // Factory helper để tạo Product với giá trị mặc định
    protected static Product CreateFakeProduct(
        ulong? id = null,
        string name = "Rose Bouquet",
        ulong? categoryId = null)
    {
        var pid = id ?? 1;
        var cid = categoryId ?? 1;

        return new Product
        {
            Id = pid,
            Name = name,
            Description = "Beautiful roses",
            Price = 100_000,
            OriginalPrice = 120_000,
            IsContactPrice = false,
            CategoryId = cid,
            FileAttachments = new List<FileAttachment>
        {
            new FileAttachment
            {
                Id = 1,
                PublicId = "products/abc123",
                SecureUrl = "https://res.cloudinary.com/demo/image/upload/products/abc123.jpg",
                Url = "http://res.cloudinary.com/demo/image/upload/products/abc123.jpg",
                Format = "jpg",
                Width = 800,
                Height = 600,
                Bytes = 102400,
                IsMain = true,
                SortOrder = 0
            }
        },
            ProductDetail = new ProductDetail
            {
                Id = 1,
                Sku = "SKU-001",
                Slug = "rose-bouquet",
                SizePrices = new List<ProductSizePrices>
            {
                new ProductSizePrices { Label = "S", Price = 80_000 },
                new ProductSizePrices { Label = "L", Price = 150_000 },
            }
            }
        };
    }

    protected static IFormFile CreateFakeFormFile(string fileName = "flower.jpg")
    {
        var mock = new Mock<IFormFile>();
        var content = "fake-image-bytes";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
        mock.Setup(f => f.FileName).Returns(fileName);
        mock.Setup(f => f.Length).Returns(stream.Length);
        mock.Setup(f => f.OpenReadStream()).Returns(stream);
        mock.Setup(f => f.ContentType).Returns("image/jpeg");
        return mock.Object;
    }

    protected static CloudinaryUploadResult SuccessUploadResult(string publicId = "products/img1") =>
        new CloudinaryUploadResult
        {
            Success = true,
            PublicId = publicId,
            SecureUrl = $"https://res.cloudinary.com/{publicId}.jpg",
            Url = $"http://res.cloudinary.com/{publicId}.jpg",
            Format = "jpg",
            Width = 800,
            Height = 600,
            Bytes = 50000,
            ErrorMessage = null
        };

    protected static CloudinaryUploadResult FailedUploadResult() =>
        new CloudinaryUploadResult
        {
            Success = false,
            ErrorMessage = "Upload timeout"
        };
}
