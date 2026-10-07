namespace codesphere_api.Common.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int StatusCode { get; set; }
        public List<string>? Errors { get; set; }

        public ApiResponse() { }

        private ApiResponse(bool success, string message, T? data, int statusCode, List<string>? errors = null)
        {
            Success = success;
            Message = message;
            Data = data;
            StatusCode = statusCode;
            Errors = errors;
        }

        public static ApiResponse<T> Ok(T? data, string message = "Request successful.", int statusCode = StatusCodes.Status200OK)
        {
            return new ApiResponse<T>(true, message, data, statusCode);
        }

        public static ApiResponse<T> Fail(string message, int statusCode = StatusCodes.Status400BadRequest, List<string>? errors = null)
        {
            return new ApiResponse<T>(false, message, default, statusCode, errors);
        }
    }


}
