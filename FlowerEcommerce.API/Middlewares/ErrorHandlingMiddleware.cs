namespace FlowerEcommerce.API.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var (statusCode, errorCode, message) = exception switch
        {
            ValidationException validationException => (StatusCodes.Status400BadRequest, ErrorCodes.BAD_REQUEST, "One or more validation errors occurred."),
            ForbiddenAccessException => (StatusCodes.Status403Forbidden, ErrorCodes.FORBIDDEN, "You are not authorized to perform this action."),
            UnauthorizedAccessException unauthorizedAccessException => (StatusCodes.Status401Unauthorized, ErrorCodes.UNAUTHORIZED, unauthorizedAccessException.Message),
            KeyNotFoundException keyNotFoundException => (StatusCodes.Status404NotFound, ErrorCodes.NOT_FOUND, keyNotFoundException.Message),
            InvalidOperationException invalidOperationException => (StatusCodes.Status400BadRequest, ErrorCodes.BAD_REQUEST, invalidOperationException.Message),
            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.SERVER_ERROR, "An internal server error has occurred.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var validationErrors = (exception as ValidationException)?.Errors
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

        var response = ApiResponse<object>.Fail(
            message: message,
            errorCode: errorCode,
            validationErrors: validationErrors
        );

        return context.Response.WriteAsJsonAsync(response);
    }
}
