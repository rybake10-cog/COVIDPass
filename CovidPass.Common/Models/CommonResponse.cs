// © 2020 Partners HealthCare Technology & Innovation Department
namespace CovidPass.Common.Models
{
    public class CommonResponse
    {
        public bool IsError { get; set; }
        public string Message { get; set; }

        public CommonResponse(bool isError = false, string message = null)
        {
            IsError = isError;
            Message = message;
        }
    }

    public class CommonResponse<T>
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }

        public CommonResponse(T result, bool isError = false, string message = null)
        {
            Result = result;
            IsError = isError;
            Message = message;
        }
    }
}
