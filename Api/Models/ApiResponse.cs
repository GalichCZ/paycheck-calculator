namespace Api.Models;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    
    // Constructor to initialize with data
    public ApiResponse(T data)
    {
        Data = data;
    }

    // Method to set an error message
    public void SetError(string errorMessage)
    {
        Success = false;
        Error = errorMessage;
    }

    // Method to set a success message
    public void SetSuccess(string successMessage)
    {
        Success = true;
        Message = successMessage;
    }
}
