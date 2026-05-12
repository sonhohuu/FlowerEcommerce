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

    #region Users
    public const string UserNotFound = "Người dùng không tồn tại.";
    public const string CannotUpdateAdminStatus = "Không thể thay đổi trạng thái của quản trị viên.";
    #endregion

    #region Products
    public const string ProductNotFound = "Sản phẩm không tồn tại.";
    public const string ProductAlreadyExists = "Tên sản phẩm này đã tồn tại.";
    public const string ProductNameValidLength = "Tên sản phẩm phải từ 5 ký tự đến 100 ký tự.";
    public const string ProductDescriptionValidLength = "Mô tả sản phẩm không được vượt quá 500 ký tự";
    public const string ProductPriceValid = "Giá sản phẩm phải lớn hơn hoặc bằng 0";
    #endregion

    #region ProductRatings
    public const string ProductRatingScoreValid = "Điểm đánh giá phải nằm trong khoảng từ 1 đến 5.";
    public const string ProductRatingCommentValidLength = "Nội dung đánh giá không được vượt quá 1000 ký tự.";
    public const string ProductRatingAlreadyExists = "Bạn đã đánh giá sản phẩm này rồi.";
    public const string ProductRatingNotFound = "Đánh giá sản phẩm không tồn tại.";
    #endregion

    #region Categories
    public const string CategoryNotFound = "Danh mục không tồn tại.";
    public const string CategoryAlreadyExists = "Tên danh mục này đã tồn tại.";
    public const string CategoryNameValidLength = "Tên danh mục phải từ 3 ký tự đến 50 ký tự.";
    #endregion

    #region Orders
    public const string OrderNotFound = "Đơn hàng không tồn tại.";
    public const string CustomerNameValidLength = "Tên khách hàng phải từ 5 ký tự đến 100 ký tự.";
    public const string PhoneNumberValid = "Số điện thoại không hợp lệ.";
    public const string AddressValidLength = "Địa chỉ phải từ 10 ký tự đến 200 ký tự.";
    public const string OrderItemQuantity = "Số lượng phải lớn hơn 0.";
    public const string OrderItemsRequired = "Danh sách sản phẩm trong đơn hàng không được để trống .";
    public const string OrderItemNotFound = "Một hoặc nhiều sản phẩm trong đơn hàng không tồn tại.";
    public const string OrderCannotBeUpdated = "Không thể cập nhật đơn hàng đã hoàn thành hoặc thất bại.";
    public const string InvalidStatusTransition = "Chuyển đổi trạng thái không hợp lệ.";
    public const string OrderCannotBeDeleted = "Không thể xóa đơn hàng đã hoàn thành hoặc thất bại.";
    public const string OrderCannotBeCancelled = "Không thể hủy đơn khi trạng thái không phải xác nhận.";
    #endregion

    #region Files
    public const string ImageUploadFailed = "Tải lên hình ảnh thất bại.";
    #endregion

}
