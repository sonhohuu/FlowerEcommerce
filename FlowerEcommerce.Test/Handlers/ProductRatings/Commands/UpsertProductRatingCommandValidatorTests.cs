using FlowerEcommerce.Application.Handlers.ProductRatings.Commands.UpsertProductRating;

namespace FlowerEcommerce.Test.Handlers.ProductRatings.Commands;

public class UpsertProductRatingCommandValidatorTests
{
    private readonly UpsertProductRatingCommandValidator _validator = new();

    // ── ProductId ──
    [Theory]
    [InlineData(1UL)]
    [InlineData(999UL)]
    public void ProductId_Valid_ShouldPass(ulong productId)
    {
        var command = new UpsertProductRatingCommand { ProductId = productId, Score = 3, Comment = "Tốt" };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.ProductId);
    }

    [Fact]
    public void ProductId_Zero_ShouldFail()
    {
        var command = new UpsertProductRatingCommand { ProductId = 0UL, Score = 3, Comment = "Tốt" };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.ProductId)
                  .WithErrorMessage("ProductId không hợp lệ.");
    }

    // ── Score ──
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Score_Valid_ShouldPass(int score)
    {
        var command = new UpsertProductRatingCommand { ProductId = 1, Score = score, Comment = "Tốt" };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Score);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(100)]
    public void Score_OutOfRange_ShouldFail(int score)
    {
        var command = new UpsertProductRatingCommand { ProductId = 1, Score = score, Comment = "Tốt" };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.Score)
                  .WithErrorMessage(MessageKey.ProductRatingScoreValid);
    }

    // ── Comment ──
    [Fact]
    public void Comment_Valid_ShouldPass()
    {
        var command = new UpsertProductRatingCommand { ProductId = 1, Score = 3, Comment = "Sản phẩm rất tốt" };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Comment);
    }

    [Fact]
    public void Comment_ExactMaxLength_ShouldPass()
    {
        var command = new UpsertProductRatingCommand { ProductId = 1, Score = 3, Comment = new string('A', 1000) };
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.Comment);
    }

    [Fact]
    public void Comment_TooLong_ShouldFail()
    {
        var command = new UpsertProductRatingCommand { ProductId = 1, Score = 3, Comment = new string('A', 1001) };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.Comment)
                  .WithErrorMessage(MessageKey.ProductRatingCommentValidLength);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Comment_NullOrEmpty_ShouldFail(string? comment)
    {
        var command = new UpsertProductRatingCommand { ProductId = 1, Score = 3, Comment = comment };
        _validator.TestValidate(command)
                  .ShouldHaveValidationErrorFor(x => x.Comment)
                  .WithErrorMessage("Nội dung đánh giá không được để trống.");
    }
}
