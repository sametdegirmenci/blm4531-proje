namespace CarRentalSystem.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public DateTime Timestamp { get; set; }

    public ApiResponse(T? data, string message = "", bool success = true)
    {
        Success = success;
        Message = message;
        Data = data;
        Timestamp = DateTime.UtcNow;
    }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
    {
        return new ApiResponse<T>(data, message, true);
    }

    public static ApiResponse<T> ErrorResponse(string message)
    {
        return new ApiResponse<T>(default, message, false);
    }
}
