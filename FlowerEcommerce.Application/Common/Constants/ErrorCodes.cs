namespace FlowerEcommerce.Domain.Constants;
public enum ErrorCodes
{
    BAD_REQUEST = 400,
    UNAUTHORIZED = 401,
    FORBIDDEN = 403,
    NOT_FOUND = 404,
    NOT_ACTIVE = 405,
    CHANGE_PASSWORD = 406,
    TIME_OUT = 408,
    ALREADY_EXISTS = 409,
    CHOOSE_ACCOUNT = 410,
    UNPROCESSABLE_ENTITY = 422,
    SERVER_ERROR = 500
}

public static class MessageKey
{
    #region Common
    public const string InternalError = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại.";
    public const string ValidationFailed = "Dữ liệu không hợp lệ.";
    public const string Unauthorized = "Bạn chưa đăng nhập.";
    public const string Forbidden = "Bạn không có quyền truy cập.";
    #endregion

    #region Products
    public const string ProductNotFound = "Sản phẩm không tồn tại.";
    public const string ProductAlreadyExists = "Tên sản phẩm này đã tồn tại.";
    public const string ProductNameValidLength = "Tên sản phẩm không được vượt quá 100 ký tự.";
    public const string ProductDescriptionValidLength = "Mô tả sản phẩm không được vượt quá 500 ký tự";
    public const string ProductPriceValid = "Giá sản phẩm phải lớn hơn 0";
    #endregion

    #region Categories
    public const string CategoryNotFound = "Danh mục không tồn tại.";
    #endregion

    #region Files
    public const string ImageUploadFailed = "Tải lên hình ảnh thất bại.";
    #endregion

}
