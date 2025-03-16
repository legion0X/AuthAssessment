namespace Assessment.Application.Common
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int StatusCode { get; set; }

        public BaseResponse(bool success, string message, T? data, int statusCode)
        {
            Success = success;
            Message = message;
            Data = data;
            StatusCode = statusCode;
        }

        public static BaseResponse<T> SuccessResponse(T data, string message = "Success", int statusCode = 200)
        {
            return new BaseResponse<T>(true, message, data, statusCode);
        }

        public static BaseResponse<T> FailureResponse(string message, int statusCode = 400)
        {
            return new BaseResponse<T>(false, message, default, statusCode);
        }
    }
}
